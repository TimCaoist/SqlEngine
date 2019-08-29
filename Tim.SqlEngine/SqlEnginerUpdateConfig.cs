using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;

namespace Tim.SqlEngine
{
    public static partial class SqlEnginerConfig
    {
        private readonly static List<UpdateRule> updateRules = new List<UpdateRule>();

        private static bool loadedRules = false;

        public static void LoadUpdateRules() {
            Task.Factory.StartNew(() => {
                var ruleDict = string.Concat(SqlEnginerConfig.ConfigFolder, !SqlEnginerConfig.ConfigFolder.EndsWith("\\") ? "\\" : string.Empty, "rules");
                if (!Directory.Exists(ruleDict))
                {
                    return;
                }

                lock (updateRules)
                {
                    loadedRules = false;
                    updateRules.Clear();
                    var files = Directory.GetFiles(ruleDict, string.Concat("*", JsonParser.Json), SearchOption.AllDirectories);
                    foreach(var file in files){
                        updateRules.AddRange(JsonParser.ReadHandlerConfig<IEnumerable<UpdateRule>>(file));
                    }
                }

                loadedRules = true;
            });
        }

        public static IEnumerable<UpdateRule> GetMatchRules(string dbName, string table)
        {
            if (loadedRules)
            {
                return updateRules.Where(r => r.RangeType == RangeType.All ||
                                   (r.RangeType == RangeType.DBs && r.DBKeys != null && r.DBKeys.Contains(dbName)) ||
                                   (r.RangeType == RangeType.Tabels && r.DBKeys != null && r.DBKeys.Contains(dbName) && r.Tables != null && r.Tables.Contains(table))
                             ).ToArray();
            }

            lock (updateRules)
            {
                return updateRules.Where(r => r.RangeType == RangeType.All ||
                                (r.RangeType == RangeType.DBs && r.DBKeys != null && r.DBKeys.Contains(dbName)) ||
                                (r.RangeType == RangeType.Tabels && r.DBKeys != null && r.DBKeys.Contains(dbName) && r.Tables != null && r.Tables.Contains(table))
                          ).ToArray();
            }
        }
    }
}
