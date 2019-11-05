using System;
using System.Collections.Generic;
using System.Configuration;
using Tim.CacheUtil.Models;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;
using Tim.SqlEngine.PlugIn;

namespace Tim.SqlEngine
{
    public static partial class SqlEnginerConfig
    {
        private readonly static Dictionary<string, string> connectionDict = new Dictionary<string, string>();

        /// <summary>
        /// 全局变量储存
        /// </summary>
        private readonly static Dictionary<string, object> globalDatas = new Dictionary<string, object>();

        private readonly static Dictionary<string, string> mapConnections = new Dictionary<string, string>();

        private static string configFolder;

        private static readonly string dataSource = "Data Source=";

        static SqlEnginerConfig()
        {
            CacheUtil.CacheUtil.UseCacheKeyCreator(new CacheNameCreator());
        }

        public static string ConfigFolder {
            get {
                return configFolder;
            }
            set {
                if (!System.IO.Directory.Exists(value))
                {
                    throw new ArgumentException("文件夹不存在!");
                }

                configFolder = value;
                LoadUpdateRules();
                LoadMapConnections();
                LoadCacheConfig();
            }
        }

        private static void LoadCacheConfig()
        {
            var cacheConfig = string.Concat(SqlEnginerConfig.ConfigFolder, !SqlEnginerConfig.ConfigFolder.EndsWith("\\") ? "\\" : string.Empty, "configs\\cache.json");
            if (!System.IO.File.Exists(cacheConfig))
            {
                return;
            }

            var cache = JsonParser.ReadHandlerConfig<CacheCommonConfig>(cacheConfig);
            CacheUtil.CacheUtil.ApplyConfig(cache);
        }

        private static void LoadMapConnections()
        {
            mapConnections.Clear();
            var mapconfig = string.Concat(SqlEnginerConfig.ConfigFolder, !SqlEnginerConfig.ConfigFolder.EndsWith("\\") ? "\\" : string.Empty, "configs\\map.json");
            if (!System.IO.File.Exists(mapconfig))
            {
                return;
            }

            var maps = JsonParser.ReadHandlerConfig<IEnumerable<Template>>(mapconfig);
            foreach (var map in maps)
            {
                mapConnections.Add(map.Name, map.Value);
                string val = string.Empty;
                if (!connectionDict.TryGetValue(map.Name, out val))
                {
                    continue;
                }

                connectionDict.Remove(map.Name);
                connectionDict.Add(map.Value, val);
            }
        }

        public static void RegisterConnection(string key, string connectionStr)
        {
            if (connectionDict.ContainsKey(key))
            {
                connectionDict[key] = connectionStr;
                return;
            }

            connectionDict.Add(key, connectionStr);
        }

        public static string GetConnection(string connectionStr)
        {
            var str = string.Empty;
            if (connectionDict.TryGetValue(connectionStr, out str))
            {
                return str;
            }

            return connectionStr;
        }

        public static void RegisterGlobalDatas(string key, object data) {
            if (globalDatas.ContainsKey(key))
            {
                return;
            }

            globalDatas.Add(key, data);
        }

        public static object GetGlobalDatas(string key)
        {
            object data;
            if (globalDatas.TryGetValue(key, out data))
            {
                return data;
            }

            return null;
        }

        public static void RegiserConnectionByConfigurationManager()
        {
            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
            {
                if (!item.ConnectionString.StartsWith(dataSource, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string alias = string.Empty;
                if (mapConnections.TryGetValue(item.Name, out alias))
                {
                    SqlEnginerConfig.RegisterConnection(alias, item.ConnectionString);
                    continue;
                }

                SqlEnginerConfig.RegisterConnection(item.Name, item.ConnectionString);
            }
        }
    }
}
