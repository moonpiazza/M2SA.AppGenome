using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Globalization;
using M2SA.AppGenome.Logging;

namespace M2SA.AppGenome.Cache.MemCached
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class MemcachedProxy
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly static string MemcachedLogger = "Memcached";

        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private string keyPrefix = "";
        private static ILog logger = LogManager.GetLogger(MemcachedLogger);

        private static int getUnixTime(DateTime datetime)
        {
            return (int)(datetime.ToUniversalTime() - epoch).TotalSeconds;
        }

        private int compressionThreshold = 1024 * 8; //128kb
        private readonly ServerPool serverPool;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scope"></param>
        /// <param name="servers"></param>
        internal MemcachedProxy(string name, ServerScope scope, IList<ServerNode> servers)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ConfigurationErrorsException("Name of MemcachedClient instance cannot be empty.");
            }
            if ((servers == null) || (servers.Count == 0))
            {
                throw new ConfigurationErrorsException("Cannot configure MemcachedClient with empty list of hosts.");
            }
            this.Name = name;
            this.KeyPrefix = string.Format("_{0}_", name);
            this.serverPool = new ServerPool(servers, scope);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(string key, object value)
        {
            return this.store("add", key, true, value, this.hash(key), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Add(string key, object value, DateTime expiry)
        {
            return this.store("add", key, true, value, this.hash(key), getUnixTime(expiry));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool Add(string key, object value, string region)
        {
            return this.store("add", key, false, value, this.hash(region), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Add(string key, object value, TimeSpan expiry)
        {
            return this.store("add", key, true, value, this.hash(key), (int) expiry.TotalSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool Add(string key, object value, uint hash)
        {
            return this.store("add", key, false, value, this.hash(hash), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Add(string key, object value, uint hash, DateTime expiry)
        {
            return this.store("add", key, false, value, this.hash(hash), getUnixTime(expiry));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Add(string key, object value, uint hash, TimeSpan expiry)
        {
            return this.store("add", key, false, value, this.hash(hash), (int) expiry.TotalSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Append(string key, object value)
        {
            return this.store("append", key, true, value, this.hash(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool Append(string key, object value, uint hash)
        {
            return this.store("append", key, false, value, this.hash(hash));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public CasResult CheckAndSet(string key, object value, ulong unique)
        {
            return this.store(key, true, value, this.hash(key), 0, unique);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public CasResult CheckAndSet(string key, object value, DateTime expiry, ulong unique)
        {
            return this.store(key, true, value, this.hash(key), getUnixTime(expiry), unique);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public CasResult CheckAndSet(string key, object value, TimeSpan expiry, ulong unique)
        {
            return this.store(key, true, value, this.hash(key), (int) expiry.TotalSeconds, unique);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public CasResult CheckAndSet(string key, object value, uint hash, ulong unique)
        {
            return this.store(key, false, value, this.hash(hash), 0, unique);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="expiry"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public CasResult CheckAndSet(string key, object value, uint hash, DateTime expiry, ulong unique)
        {
            return this.store(key, false, value, this.hash(hash), getUnixTime(expiry), unique);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="expiry"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public CasResult CheckAndSet(string key, object value, uint hash, TimeSpan expiry, ulong unique)
        {
            return this.store(key, false, value, this.hash(hash), (int) expiry.TotalSeconds, unique);
        }

        private void checkKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("Key may not be null.");
            }
            if (key.Length == 0)
            {
                throw new ArgumentException("Key may not be empty.");
            }
            if (key.Length > 250)
            {
                throw new ArgumentException("Key may not be longer than 250 characters.");
            }
            if ((((key.Contains(" ") || key.Contains("\n")) || (key.Contains("\r") || key.Contains("\t"))) || key.Contains("\f")) || key.Contains("\v"))
            {
                throw new ArgumentException("Key may not contain whitespace or control characters.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ulong? Decrement(string key, ulong value)
        {
            return this.incrementDecrement("decr", key, true, value, this.hash(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public ulong? Decrement(string key, ulong value, uint hash)
        {
            return this.incrementDecrement("decr", key, false, value, this.hash(hash));
        }

        private bool delete(string key, bool keyIsChecked, uint hash, int time)
        {
            if (!keyIsChecked)
            {
                this.checkKey(key);
            }
            return this.serverPool.Execute<bool>(hash, false, delegate (PooledSocket socket) {
                string commandline;
                if (time == 0)
                {
                    commandline = "delete " + this.keyPrefix + key + "\r\n";
                }
                else
                {
                    commandline = string.Concat(new object[] { "delete ", this.keyPrefix, key, " ", time, "\r\n" });
                }
                socket.Write(commandline);
                return socket.ReadResponse().StartsWith("DELETED");
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            return this.delete(key, true, this.hash(key), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public bool Delete(string key, DateTime delay)
        {
            return this.delete(key, true, this.hash(key), getUnixTime(delay));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool Delete(string key, string region)
        {
            return this.delete(key, false, this.hash(region), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public bool Delete(string key, TimeSpan delay)
        {
            return this.delete(key, true, this.hash(key), (int) delay.TotalSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool Delete(string key, uint hash)
        {
            return this.delete(key, false, this.hash(hash), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public bool Delete(string key, uint hash, DateTime delay)
        {
            return this.delete(key, false, this.hash(hash), getUnixTime(delay));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public bool Delete(string key, uint hash, TimeSpan delay)
        {
            return this.delete(key, false, this.hash(hash), (int) delay.TotalSeconds);
        }
  
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool FlushAll()
        {
            return this.FlushAll(TimeSpan.Zero, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public bool FlushAll(TimeSpan delay)
        {
            return this.FlushAll(delay, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="staggered"></param>
        /// <returns></returns>
        public bool FlushAll(TimeSpan delay, bool staggered)
        {
            bool noerrors = true;
            uint count = 0;
            foreach (SocketPool pool in this.serverPool.HostList)
            {
                this.serverPool.Execute(pool, delegate (PooledSocket socket) {
                    uint delaySeconds = staggered ? (((uint) delay.TotalSeconds) * count) : ((uint) delay.TotalSeconds);
                    socket.Write("flush_all " + ((delaySeconds == 0) ? "" : delaySeconds.ToString()) + "\r\n");
                    if (!socket.ReadResponse().StartsWith("OK"))
                    {
                        noerrors = false;
                    }
                    count++;
                });
            }
            return noerrors;
        }

        private object get(string command, string key, bool keyIsChecked, uint hash, out ulong unique)
        {
            if (!keyIsChecked)
            {
                this.checkKey(key);
            }
            ulong __unique = 0L;
            object value = this.serverPool.Execute<object>(hash, null, delegate (PooledSocket socket) {
                object _value;
                ulong _unique;
                socket.Write(command + " " + this.keyPrefix + key + "\r\n");
                if (this.readValue(socket, out _value, out key, out _unique))
                {
                    socket.ReadLine();
                }
                __unique = _unique;
                return _value;
            });
            unique = __unique;
            return value;
        }

        private object[] get(string command, string[] keys, bool keysAreChecked, uint[] hashes, out ulong[] uniques)
        {
            			//Check arguments.
			if (keys == null || hashes == null) {
				throw new ArgumentException("Keys and hashes arrays must not be null.");
			}
			if (keys.Length != hashes.Length) {
				throw new ArgumentException("Keys and hashes arrays must be of the same length.");
			}
			uniques = new ulong[keys.Length];

			//Avoid going through the server grouping if there's only one key.
			if (keys.Length == 1) {
				return new object[] { get(command, keys[0], keysAreChecked, hashes[0], out uniques[0]) };
			}

			//Check keys.
			if (!keysAreChecked) {
				for (int i = 0; i < keys.Length; i++) {
					checkKey(keys[i]);
				}
			}

			//Group the keys/hashes by server(pool)
			Dictionary<SocketPool, Dictionary<string, List<int>>> dict = new Dictionary<SocketPool, Dictionary<string, List<int>>>();
			for (int i = 0; i < keys.Length; i++) {
				Dictionary<string, List<int>> getsForServer;
				SocketPool pool = serverPool.GetSocketPool(hashes[i]);
				if (!dict.TryGetValue(pool, out getsForServer)) {
					dict[pool] = getsForServer = new Dictionary<string, List<int>>();
				} 

				List<int> positions;
				if(!getsForServer.TryGetValue(keys[i], out positions)){
					getsForServer[keys[i]] = positions = new List<int>();
				}
				positions.Add(i);
			}

			//Get the values
			object[] returnValues = new object[keys.Length];
			ulong[] _uniques = new ulong[keys.Length];
			foreach (KeyValuePair<SocketPool, Dictionary<string, List<int>>> kv in dict) {
				serverPool.Execute(kv.Key, delegate(PooledSocket socket){
					//Build the get request
					StringBuilder getRequest = new StringBuilder(command);
					foreach (KeyValuePair<string, List<int>> key in kv.Value) {
						getRequest.Append(" ");
						getRequest.Append(this.keyPrefix);
						getRequest.Append(key.Key);
					}
					getRequest.Append("\r\n");

					//Send get request
					socket.Write(getRequest.ToString());

					//Read values, one by one
					object gottenObject;
					string gottenKey;
					ulong unique;
					while (readValue(socket, out gottenObject, out gottenKey, out unique)) {
                        var sourceKey = gottenKey.Substring(this.keyPrefix.Length);
                        foreach (int position in kv.Value[sourceKey])
                        {
							returnValues[position] = gottenObject;
							_uniques[position] = unique;
						}
					}
				});
			}
			uniques = _uniques;
			return returnValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            ulong i;
            return this.get("get", key, true, this.hash(key), out i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public object[] Get(string[] keys)
        {
            ulong[] uniques;
            return this.get("get", keys, true, this.hash(keys), out uniques);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public object[] Get(string[] keys, uint[] hashes)
        {
            ulong[] uniques;
            return this.get("get", keys, false, this.hash(hashes), out uniques);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public object Get(string key, string region)
        {
            ulong i;
            return this.get("get", key, false, this.hash(region), out i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public object Get(string key, uint hash)
        {
            ulong i;
            return this.get("get", key, false, this.hash(hash), out i);
        }

        private ulong? getCounter(string key, bool keyIsChecked, uint hash)
        {
            ulong parsedLong;
            ulong unique;
            return (ulong.TryParse(this.get("get", key, keyIsChecked, hash, out unique) as string, out parsedLong) ? new ulong?(parsedLong) : null);
        }

        private ulong?[] getCounter(string[] keys, bool keysAreChecked, uint[] hashes)
        {
            ulong[] uniques;
            ulong?[] results = new ulong?[keys.Length];
            object[] values = this.get("get", keys, keysAreChecked, hashes, out uniques);
            for (int i = 0; i < values.Length; i++)
            {
                ulong parsedLong;
                results[i] = ulong.TryParse(values[i] as string, out parsedLong) ? new ulong?(parsedLong) : null;
            }
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ulong? GetCounter(string key)
        {
            return this.getCounter(key, true, this.hash(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public ulong?[] GetCounter(string[] keys)
        {
            return this.getCounter(keys, true, this.hash(keys));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public ulong? GetCounter(string key, uint hash)
        {
            return this.getCounter(key, false, this.hash(hash));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public ulong?[] GetCounter(string[] keys, uint[] hashes)
        {
            return this.getCounter(keys, false, this.hash(hashes));
        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public object Gets(string key, out ulong unique)
        {
            return this.get("gets", key, true, this.hash(key), out unique);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="uniques"></param>
        /// <returns></returns>
        public object[] Gets(string[] keys, out ulong[] uniques)
        {
            return this.get("gets", keys, true, this.hash(keys), out uniques);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        public object Gets(string key, uint hash, out ulong unique)
        {
            return this.get("gets", key, false, this.hash(hash), out unique);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="hashes"></param>
        /// <param name="uniques"></param>
        /// <returns></returns>
        public object[] Gets(string[] keys, uint[] hashes, out ulong[] uniques)
        {
            return this.get("gets", keys, false, this.hash(hashes), out uniques);
        }


        private uint hash(uint hashvalue)
        {
            byte[] bytes = null;
            using(var fnv1 = new ModifiedFNV1_32())
            {
                bytes = fnv1.ComputeHash(BitConverter.GetBytes(hashvalue));
            };
            return BitConverter.ToUInt32(bytes, 0);
        }

        private uint[] hash(string[] keys)
        {
            uint[] result = new uint[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                result[i] = this.hash(keys[i]);
            }
            return result;
        }

        private uint[] hash(uint[] hashvalues)
        {
            uint[] result = new uint[hashvalues.Length];
            for (int i = 0; i < hashvalues.Length; i++)
            {
                result[i] = this.hash(hashvalues[i]);
            }
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        private uint hash(string key)
        {
            this.checkKey(key);
            return BitConverter.ToUInt32(new ModifiedFNV1_32().ComputeHash(Encoding.UTF8.GetBytes(key)), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ulong? Increment(string key, ulong value)
        {
            return this.incrementDecrement("incr", key, true, value, this.hash(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public ulong? Increment(string key, ulong value, uint hash)
        {
            return this.incrementDecrement("incr", key, false, value, this.hash(hash));
        }

		private ulong? incrementDecrement(string cmd, string key, bool keyIsChecked, ulong value, uint hash) {
			if (!keyIsChecked) {
				checkKey(key);
			}
			return serverPool.Execute<ulong?>(hash, null, delegate(PooledSocket socket) {
				string command = cmd + " " + keyPrefix + key + " " + value + "\r\n";
				socket.Write(command);
				string response = socket.ReadResponse();
				if (response.StartsWith("NOT_FOUND")) {
					return null;
				} else {
					return Convert.ToUInt64(response.TrimEnd('\0', '\r', '\n'));
				}
			});
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Prepend(string key, object value)
        {
            return this.store("prepend", key, true, value, this.hash(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool Prepend(string key, object value, uint hash)
        {
            return this.store("prepend", key, false, value, this.hash(hash));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool readValue(PooledSocket socket, out object value, out string key, out ulong unique)
        {
            string[] parts = socket.ReadResponse().Split(new char[] { ' ' });
            if (parts[0] == "VALUE")
            {
                key = parts[1];
                SerializedType type = (SerializedType) Enum.Parse(typeof(SerializedType), parts[2]);
                byte[] bytes = new byte[Convert.ToUInt32(parts[3], CultureInfo.InvariantCulture)];
                if (parts.Length > 4)
                {
                    unique = Convert.ToUInt64(parts[4]);
                }
                else
                {
                    unique = 0L;
                }
                socket.Read(bytes);
                socket.SkipUntilEndOfLine();
                try
                {
                    value = Serializer.DeSerialize(bytes, type);
                }
                catch (Exception e)
                {
                    value = null;
                    logger.Error(string.Concat(new object[] { "Error deserializing object for key '", key, "' of type ", type, "." }), e);
                }
                return true;
            }
            key = null;
            value = null;
            unique = 0L;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Replace(string key, object value)
        {
            return this.store("replace", key, true, value, this.hash(key), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Replace(string key, object value, DateTime expiry)
        {
            return this.store("replace", key, true, value, this.hash(key), getUnixTime(expiry));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Replace(string key, object value, TimeSpan expiry)
        {
            return this.store("replace", key, true, value, this.hash(key), (int) expiry.TotalSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool Replace(string key, object value, uint hash)
        {
            return this.store("replace", key, false, value, this.hash(hash), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Replace(string key, object value, uint hash, DateTime expiry)
        {
            return this.store("replace", key, false, value, this.hash(hash), getUnixTime(expiry));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Replace(string key, object value, uint hash, TimeSpan expiry)
        {
            return this.store("replace", key, false, value, this.hash(hash), (int) expiry.TotalSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(string key, object value)
        {
            return this.store("set", key, true, value, this.hash(key), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Set(string key, object value, DateTime expiry)
        {
            return this.store("set", key, true, value, this.hash(key), getUnixTime(expiry));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool Set(string key, object value, string region)
        {
            return this.store("set", key, true, value, this.hash(region), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Set(string key, object value, TimeSpan expiry)
        {
            return this.store("set", key, true, value, this.hash(key), (int) expiry.TotalSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool Set(string key, object value, uint hash)
        {
            return this.store("set", key, false, value, this.hash(hash), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="region"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Set(string key, object value, string region, DateTime expiry)
        {
            return this.store("set", key, true, value, this.hash(region), getUnixTime(expiry));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="region"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Set(string key, object value, string region, TimeSpan expiry)
        {
            return this.store("set", key, true, value, this.hash(region), (int) expiry.TotalSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Set(string key, object value, uint hash, DateTime expiry)
        {
            return this.store("set", key, false, value, this.hash(hash), getUnixTime(expiry));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Set(string key, object value, uint hash, TimeSpan expiry)
        {
            return this.store("set", key, false, value, this.hash(hash), (int) expiry.TotalSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetCounter(string key, ulong value)
        {
            return this.Set(key, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool SetCounter(string key, ulong value, DateTime expiry)
        {
            return this.Set(key, value.ToString(CultureInfo.InvariantCulture), expiry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool SetCounter(string key, ulong value, TimeSpan expiry)
        {
            return this.Set(key, value.ToString(CultureInfo.InvariantCulture), expiry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool SetCounter(string key, ulong value, uint hash)
        {
            return this.Set(key, value.ToString(CultureInfo.InvariantCulture), this.hash(hash));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool SetCounter(string key, ulong value, uint hash, DateTime expiry)
        {
            return this.Set(key, value.ToString(CultureInfo.InvariantCulture), this.hash(hash), expiry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool SetCounter(string key, ulong value, uint hash, TimeSpan expiry)
        {
            return this.Set(key, value.ToString(CultureInfo.InvariantCulture), this.hash(hash), expiry);
        }
        private IDictionary<string, string> stats(SocketPool pool)
        {
            if (pool == null)
            {
                return null;
            }
            IDictionary<string, string> result = new Dictionary<string, string>();
            serverPool.Execute(pool, delegate(PooledSocket socket)
            {
                socket.Write("stats\r\n");
                string line;
                while (!(line = socket.ReadResponse().TrimEnd('\0', '\r', '\n')).StartsWith("END"))
                {
                    string[] s = line.Split(' ');
                    result.Add(s[1], s[2]);
                }
            });
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, IDictionary<string, string>> Stats()
        {
            IDictionary<string, IDictionary<string, string>> results = new Dictionary<string, IDictionary<string, string>>();
            foreach (SocketPool pool in this.serverPool.HostList)
            {
                results.Add(pool.Host, this.stats(pool));
            }
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IDictionary<string, string> Stats(string key)
        {
            return this.Stats(this.hash(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public IDictionary<string, string> Stats(uint hash)
        {
            return this.stats(this.serverPool.GetSocketPool(this.hash(hash)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public IDictionary<string, string> StatsByHost(string host)
        {
            return this.stats(this.serverPool.GetSocketPool(host));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, IDictionary<string, string>> Status()
        {
            IDictionary<string, IDictionary<string, string>> results = new Dictionary<string, IDictionary<string, string>>();
            foreach (SocketPool pool in this.serverPool.HostList)
            {
                IDictionary<string, string> result = new Dictionary<string, string>();
                if (this.serverPool.Execute<bool>(pool, false, delegate {
                    return true;
                }))
                {
                    result.Add("Status", "Ok");
                }
                else
                {
                    result.Add("Status", "Dead, next retry at: " + pool.DeadEndPointRetryTime);
                }
                result.Add("Sockets in pool", pool.Poolsize.ToString());
                result.Add("Acquired sockets", pool.Acquired.ToString());
                result.Add("Sockets reused", pool.ReusedSockets.ToString());
                result.Add("New sockets created", pool.NewSockets.ToString());
                result.Add("New sockets failed", pool.FailedNewSockets.ToString());
                result.Add("Sockets died in pool", pool.DeadSocketsInPool.ToString());
                result.Add("Sockets died on return", pool.DeadSocketsOnReturn.ToString());
                result.Add("Dirty sockets on return", pool.DirtySocketsOnReturn.ToString());
                results.Add(pool.Host, result);
            }
            return results;
        }

        private bool store(string command, string key, bool keyIsChecked, object value, uint hash)
        {
            return this.store(command, key, keyIsChecked, value, hash, 0, 0L).StartsWith("STORED");
        }

        private CasResult store(string key, bool keyIsChecked, object value, uint hash, int expiry, ulong unique)
        {
            string result = this.store("cas", key, keyIsChecked, value, hash, expiry, unique);
            if (result.StartsWith("STORED"))
            {
                return CasResult.Stored;
            }
            if (result.StartsWith("EXISTS"))
            {
                return CasResult.Exists;
            }
            if (result.StartsWith("NOT_FOUND"))
            {
                return CasResult.NotFound;
            }
            return CasResult.NotStored;
        }

        private bool store(string command, string key, bool keyIsChecked, object value, uint hash, int expiry)
        {
            return this.store(command, key, keyIsChecked, value, hash, expiry, 0L).StartsWith("STORED");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private string store(string command, string key, bool keyIsChecked, object value, uint hash, int expiry, ulong unique)
        {
            if (!keyIsChecked)
            {
                this.checkKey(key);
            }
            return this.serverPool.Execute<string>(hash, "", delegate (PooledSocket socket) {
                SerializedType type;
                byte[] bytes;
                try
                {
                    bytes = Serializer.Serialize(value, out type, (uint)this.CompressionThreshold);
                }
                catch (Exception e)
                {
                    logger.Error("Error serializing object for key '" + key + "'.", e);
                    return "";
                }

                string commandline = "";
				switch(command) {
					case "set":
					case "add":
					case "replace":
						commandline = string.Concat(new object[] { command, " ", this.keyPrefix, key, " ", (ushort) type, " ", expiry, " ", bytes.Length, "\r\n" });
						break;
					case "append":
					case "prepend":
						commandline = string.Concat(new object[] { command, " ", this.keyPrefix, key, " 0 0 ", bytes.Length, "\r\n" });
						break;
					case "cas":
						commandline = string.Concat(new object[] { command, " ", this.keyPrefix, key, " ", (ushort) type, " ", expiry, " ", bytes.Length, " ", unique, "\r\n" });
						break;
				}

                socket.Write(commandline);
                socket.Write(bytes);
                socket.Write("\r\n");
                return socket.ReadResponse();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public int CompressionThreshold
        {
            get
            {
                return this.compressionThreshold;
            }
            set
            {
                this.compressionThreshold = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string KeyPrefix
        {
            get
            {
                return this.keyPrefix;
            }
            set
            {
                this.keyPrefix = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxPoolSize
        {
            get
            {
                return this.serverPool.MaxPoolSize;
            }
            set
            {
                if (value < this.MinPoolSize)
                {
                    throw new ConfigurationErrorsException(string.Concat(new object[] { "MaxPoolSize (", value, ") may not be smaller than the MinPoolSize (", this.MinPoolSize, ")." }));
                }
                this.serverPool.MaxPoolSize = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MinPoolSize
        {
            get
            {
                return this.serverPool.MinPoolSize;
            }
            set
            {
                if (value > this.MaxPoolSize)
                {
                    throw new ConfigurationErrorsException(string.Concat(new object[] { "MinPoolSize (", value, ") may not be larger than the MaxPoolSize (", this.MaxPoolSize, ")." }));
                }
                this.serverPool.MinPoolSize = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SendReceiveTimeout
        {
            get
            {
                return this.serverPool.SendReceiveTimeout;
            }
            set
            {
                this.serverPool.SendReceiveTimeout = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan SocketRecycleAge
        {
            get
            {
                return this.serverPool.SocketRecycleAge;
            }
            set
            {
                this.serverPool.SocketRecycleAge = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum CasResult
        {
            /// <summary>
            /// 
            /// </summary>
            Stored,
            
            /// <summary>
            /// 
            /// </summary>
            NotStored,
            
            /// <summary>
            /// 
            /// </summary>
            Exists,
            
            /// <summary>
            /// 
            /// </summary>
            NotFound
        }
    }
}