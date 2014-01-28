using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Logging;

namespace M2SA.AppGenome.Cache.MemCached
{
	internal delegate T UseSocket<T>(PooledSocket socket);
	internal delegate void UseSocket(PooledSocket socket);

	/// <summary>
	/// The ServerPool encapsulates a collection of memcached servers and the associated SocketPool objects.
	/// This class contains the server-selection logic, and contains methods for executing a block of code on 
	/// a socket from the server corresponding to a given key.
	/// </summary>
	internal class ServerPool {
        private static ILog logger = LogManager.GetLogger(MemcachedProxy.MemcachedLogger);

		//Expose the socket pools.
		private SocketPool[] hostList;
		internal SocketPool[] HostList { get { return hostList; } }

		private Dictionary<uint, SocketPool> hostDictionary;
        private Dictionary<string, SocketPool[]> slavePoolMap;
		private uint[] hostKeys;

		//Internal configuration properties
		private int sendReceiveTimeout = 2000;
		private int connectTimeout = 2000;
		private int maxPoolSize = 100;
		private int minPoolSize = 50;
		private TimeSpan socketRecycleAge = TimeSpan.FromMinutes(30);
		internal int SendReceiveTimeout { get { return sendReceiveTimeout; } set { sendReceiveTimeout = value; } }
		internal int ConnectTimeout { get { return connectTimeout; } set { connectTimeout = value; } }
		internal int MaxPoolSize { get { return maxPoolSize; } set { maxPoolSize = value; } }
		internal int MinPoolSize { get { return minPoolSize; } set { minPoolSize = value; } }
		internal TimeSpan SocketRecycleAge { get { return socketRecycleAge; } set { socketRecycleAge = value; } }

        public readonly ServerScope Scope;

		/// <summary>
		/// Internal constructor. This method takes the array of hosts and sets up an internal list of socketpools.
		/// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        internal ServerPool(IList<ServerNode> servers, ServerScope scope)
        {
            this.Scope = scope;
            this.slavePoolMap = new Dictionary<string, SocketPool[]>();
			hostDictionary = new Dictionary<uint, SocketPool>();
			List<SocketPool> pools = new List<SocketPool>();
			List<uint> keys = new List<uint>();
            foreach (var server in servers)
            {
				//Create pool
                SocketPool pool = new SocketPool(this, server.Address.Trim());
                if (server.Slaves != null && server.Slaves.Count > 0)
                {
                    var slaveList = new SocketPool[server.Slaves.Count];

                    for(var i=0 ; i<server.Slaves.Count; i++)
                        slaveList[i] = new SocketPool(this, server.Slaves[i].Address.Trim());

                    this.slavePoolMap[server.Address] = slaveList;
                }                

				//Create 250 keys for this pool, store each key in the hostDictionary, as well as in the list of keys.
				for (int i = 0; i < 250; i++) {
                    uint key = BitConverter.ToUInt32(new ModifiedFNV1_32().ComputeHash(Encoding.UTF8.GetBytes(server.Address + "-" + i)), 0);
					if (!hostDictionary.ContainsKey(key)) {
						hostDictionary[key] = pool;
						keys.Add(key);
					}
				}

				pools.Add(pool);
			}

			//Hostlist should contain the list of all pools that has been created.
			hostList = pools.ToArray();

			//Hostkeys should contain the list of all key for all pools that have been created.
			//This array forms the server key continuum that we use to lookup which server a
			//given item key hash should be assigned to.
			keys.Sort();
			hostKeys = keys.ToArray();
		}

		/// <summary>
		/// Given an item key hash, this method returns the socketpool which is closest on the server key continuum.
		/// </summary>
		internal SocketPool GetSocketPool(uint hash) {
			//Quick return if we only have one host.
			if (hostList.Length == 1) {
				return hostList[0];
			}

			//New "ketama" host selection.
			int i = Array.BinarySearch(hostKeys, hash);

			//If not exact match...
			if(i < 0) {
				//Get the index of the first item bigger than the one searched for.
				i = ~i;

				//If i is bigger than the last index, it was bigger than the last item = use the first item.
				if (i >= hostKeys.Length) {
					i = 0;
				}
			}
			return hostDictionary[hostKeys[i]];
		}

		internal SocketPool GetSocketPool(string host) {
			return Array.Find(HostList, delegate(SocketPool socketPool) { return socketPool.Host == host; });
		}

		/// <summary>
		/// This method executes the given delegate on a socket from the server that corresponds to the given hash.
		/// If anything causes an error, the given defaultValue will be returned instead.
		/// This method takes care of disposing the socket properly once the delegate has executed.
		/// </summary>
		internal T Execute<T>(uint hash, T defaultValue, UseSocket<T> use) {

            var socketPool = GetSocketPool(hash);
            var result = Execute(socketPool, defaultValue, use);

            if (this.Scope == ServerScope.Write && this.slavePoolMap.ContainsKey(socketPool.Host))
            {
                var slaves = this.slavePoolMap[socketPool.Host];
                foreach (var slaveSocket in slaves)
                {
                    //AppInstance.GetThreadPool().QueueWorkItem(
                    Action action = () => {                        
                        LogManager.GetLogger(MemcachedProxy.MemcachedLogger).Debug("Sync Slave {0} By Master:{1}", slaveSocket.Host, socketPool.Host);
                        Execute(slaveSocket, defaultValue, use);
                    };

                    AppInstance.GetThreadPool().QueueWorkItem(action);
                }              
            }
            return result;
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal T Execute<T>(SocketPool pool, T defaultValue, UseSocket<T> use) {
			PooledSocket sock = null;
			try {
				//Acquire a socket
				sock = pool.Acquire();

				//Use the socket as a parameter to the delegate and return its result.
				if (sock != null) {
					return use(sock);
				}
			} catch(Exception e) {
				logger.Error("Error in Execute<T>: " + pool.Host, e);

				//Socket is probably broken
				if (sock != null) {
					sock.Close();
				}
			} finally {
				if (sock != null) {
					sock.Dispose();
				}
			}
			return defaultValue;
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:不要多次释放对象"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal void Execute(SocketPool pool, UseSocket use) {
			PooledSocket sock = null;
			try {
				//Acquire a socket
				sock = pool.Acquire();

				//Use the socket as a parameter to the delegate and return its result.
				if (sock != null) {
					use(sock);
				}
			} catch(Exception e) {
				logger.Error("Error in Execute: " + pool.Host, e);

				//Socket is probably broken
				if (sock != null) {
					sock.Close();
				}
			}
			finally {
				if(sock != null) {
					sock.Dispose();
				}
			}
		}

		/// <summary>
		/// This method executes the given delegate on all servers.
		/// </summary>
		internal void ExecuteAll(UseSocket use) {
			foreach(SocketPool socketPool in hostList){
				Execute(socketPool, use);
			}
		}
	}
}