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

        private static Task task = null;
        private static void UpdateRules() { 
            updateRules.Clear();
            task = Task.Factory.StartNew(() => {
                var ruleDict = string.Concat(SqlEnginerConfig.ConfigFolder, !SqlEnginerConfig.ConfigFolder.EndsWith("\\") ? "\\" : string.Empty, "rules");
                if (!Directory.Exists(ruleDict))
                {
                    return;
                }

                var files = Directory.GetFiles(ruleDict, string.Concat("*", JsonParser.Json), SearchOption.AllDirectories);
                Parallel.ForEach(files, file => {
                    updateRules.AddRange(JsonParser.ReadHandlerConfig<IEnumerable<UpdateRule>>(file));
                });

                task = null;
            });
        }

        public static IEnumerable<UpdateRule> GetMatchRules(string dbName, string table)
        {
            if (task != null)
            {
                task.Wait();
            }

            return updateRules.Where(r => r.RangeType == RangeType.All ||
                                    (r.RangeType == RangeType.DBs && r.DBKeys != null && r.DBKeys.Contains(dbName)) ||
                                    (r.RangeType == RangeType.Tabels && r.DBKeys != null && r.DBKeys.Contains(dbName) && r.Tables != null && r.Tables.Contains(table))
                              ).ToArray();
        }
    }
}
