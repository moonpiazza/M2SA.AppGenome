using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Configuration;
using System.Reflection;
using M2SA.AppGenome.Configuration;
using M2SA.AppGenome.Reflection;


namespace M2SA.AppGenome
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectIOCFactory
    {
        private static object syncRoot = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ResolveInstance<T>()
        {
            return ResolveInstance<T>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static T ResolveInstance<T>(string alias)
        {
            Type sourceType = typeof(T);

            T instance = default(T);
            IConfigNode resolveInfo = null;
            if (sourceType.GetInterface(typeof(IResolveObject).Name) != null)
                resolveInfo = LoadResolveConfig<T>(alias);

            if (null == resolveInfo || false == resolveInfo.EnableSingleton)
            {
                instance = (T)ResolveObject(sourceType, alias, resolveInfo);
            }
            else
            {
                instance = (T)GetSingleton(sourceType, alias, resolveInfo);                
            }
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetSingleton<T>()
        {
            return GetSingleton<T>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static T GetSingleton<T>(string alias)
        {
            Type sourceType = typeof(T);

            IConfigNode resolveInfo = null;
            if (sourceType.GetInterface(typeof(IResolveObject).Name) != null)
                resolveInfo = LoadResolveConfig<T>(alias);

            var instance = (T)GetSingleton(sourceType, alias, resolveInfo); 

            return instance;
        }

        private static object GetSingleton(Type sourceType, string alias, IConfigNode resolveInfo)
        {
            var instanceKey = GetInstanceKey(sourceType, alias);
            var instance = SingletonMap.GetInstance(instanceKey);
            if (null == instance)
            {
                lock (syncRoot)
                {
                    instance = SingletonMap.GetInstance(instanceKey);
                    if (null == instance)
                    {
                        instance = ResolveObject(sourceType, alias, resolveInfo);
                        SingletonMap.SetInstance(instanceKey, instance);
                    }
                }
            }

            return instance;
        }

        private static string GetInstanceKey(Type sourceType, string alias)
        {
            var instanceKey = sourceType.FullName;
            if (string.IsNullOrEmpty(alias) == false)
                instanceKey = string.Format("{0}.{1}", instanceKey, alias);
            return instanceKey;
        }

        private static object ResolveObject(Type sourceType, string alias, IConfigNode resolveInfo)
        {
            var obj = sourceType.BuildObject(alias);
            if (null != resolveInfo)
                (obj as IResolveObject).Initialize(resolveInfo);
            return obj;
        }

        private static IConfigNode LoadResolveConfig<T>(string alias)
        {
            Type instanceType = typeof(T);
            var configName = instanceType.Name;
            if (instanceType.IsInterface && instanceType.Name.StartsWith("I"))
                configName = configName.Substring(1);
            if (instanceType.Name.EndsWith("Config"))
                configName = configName.Substring(0, configName.Length - "Config".Length);

            configName = configName.Substring(0, 1).ToLower() + configName.Substring(1);

            var configPath = configName;
            if (string.IsNullOrEmpty(alias) == false)
                configPath = string.Format("{0}[alias='{1}']", configName, alias);

            var resolveInfo = AppInstance.GetConfigNode(configPath);
            return resolveInfo;
        }

        static class SingletonMap
        {
            private static object syncRoot = new object();

            static IDictionary<string, object> objectMap = null;
            static SingletonMap()
            {
                objectMap = new Dictionary<string, object>(4);
            }

            internal static void SetInstance(string key, object instance)
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException("key");
                }

                lock (syncRoot)
                {
                    objectMap[key] = instance;
                }
            }

            internal static object GetInstance(string key)
            {
                if (string.IsNullOrEmpty(key) || objectMap.ContainsKey(key) == false)
                    return null;
                return objectMap[key];
            }
        }
    }
}
