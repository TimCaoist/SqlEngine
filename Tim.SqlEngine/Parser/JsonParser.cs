using Newtonsoft.Json;
using System;
using System.IO;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser
{
    internal static class JsonParser
    {
        internal static string Json = ".json";

        public static THandlerConfig ReadHandlerConfig<THandlerConfig>(string name)
        {
            if (string.IsNullOrEmpty(SqlEnginerConfig.ConfigFolder))
            {
                throw new ArgumentException("ConfigFolder");
            }

            var fileName = name;
            if (!fileName.EndsWith(Json))
            {
                fileName = string.Concat(fileName, Json);
            }

            var fullName = Path.Combine(SqlEnginerConfig.ConfigFolder, fileName);
            var text = File.ReadAllText(fullName);
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(fileName);
            }

            return JsonConvert.DeserializeObject<THandlerConfig>(text);
        }

        public static object CreateInstance(string data, Type type)
        {
            return JsonConvert.DeserializeObject(data, type);
        }
    }
}
