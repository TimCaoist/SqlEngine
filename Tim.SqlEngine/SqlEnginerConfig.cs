using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.SqlHelper.QueryHandler;

namespace Tim.SqlEngine
{
    public static class SqlEnginerConfig
    {
        private readonly static Dictionary<string, string> connectionDict = new Dictionary<string, string>();

        private readonly static Dictionary<string, object> globalDatas = new Dictionary<string, object>();

        private static string configFolder;

        private static readonly string dataSource = "Data Source=";

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

                SqlEnginerConfig.RegisterConnection(item.Name, item.ConnectionString);
            }
        }
    }
}
