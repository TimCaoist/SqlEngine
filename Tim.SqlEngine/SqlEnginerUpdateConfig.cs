using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static IEnumerable<UpdateRule> GetMatchRules(string dbName, string table, ActionType actionType, UpdateType updateType = UpdateType.DefaultValue)
        {
            ActionType[] filterActions;
            if (actionType == ActionType.Insert)
            {
                filterActions = new ActionType[] { ActionType.Insert, ActionType.All, ActionType.BothInsertAndUpdate };
            }
            else if (actionType == ActionType.Update)
            {
                filterActions = new ActionType[] { ActionType.Update, ActionType.All, ActionType.BothInsertAndUpdate };
            }
            else {
                filterActions = new ActionType[] { actionType };
            }

            Func<IEnumerable<UpdateRule>> getRules = () =>
            {
                return updateRules.Where(r => r.UpdateType == updateType && filterActions.Contains(r.ActionType)).
                                   Where(r => r.RangeType == RangeType.All ||
                                  (r.RangeType == RangeType.DBs && r.DBKeys != null && r.DBKeys.Contains(dbName)) ||
                                  (r.RangeType == RangeType.Tabels && r.DBKeys != null && r.DBKeys.Contains(dbName) && 
                                  r.Tables != null && r.Tables.Contains(table))
                            ).ToArray();
            };

            if (loadedRules)
            {
                return getRules();
            }

            lock (updateRules)
            {
                return getRules();
            }
        }
    }
}
