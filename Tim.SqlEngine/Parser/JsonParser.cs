using Newtonsoft.Json;
using System;
using System.IO;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser
{
    internal static class JsonParser
    {
        public static HandlerConfig ReadHandlerConfig(string name)
        {
            if (string.IsNullOrEmpty(SqlEnginerConfig.ConfigFolder))
            {
                throw new ArgumentException("ConfigFolder");
            }

            var fileName = string.Concat(name, ".json");
            var fullName = Path.Combine(SqlEnginerConfig.ConfigFolder, fileName);
            var text = File.ReadAllText(fullName);
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(fileName);
            }

            return JsonConvert.DeserializeObject<HandlerConfig>(text);
        }
    }
}
