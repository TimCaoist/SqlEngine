using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper
{
    public static class TranHelper
    {
        public static Tuple<MySqlConnection, MySqlTransaction> OpenTran(this UpdateContext updateContext, string connectionStr)
        {
            if (updateContext.Conns == null)
            {
                updateContext.Conns = new Dictionary<string, Tuple<MySqlConnection, MySqlTransaction>>();
            }

            Tuple<MySqlConnection, MySqlTransaction> conn;
            if (!updateContext.Conns.TryGetValue(connectionStr, out conn))
            {
                var mySqlConnectiom = new MySqlConnection(SqlEnginerConfig.GetConnection(connectionStr));
                mySqlConnectiom.Open();
                MySqlTransaction transaction = mySqlConnectiom.BeginTransaction();
                conn = Tuple.Create(mySqlConnectiom, transaction);
                updateContext.Conns.Add(connectionStr, conn);
            }

            return conn;
        }

        public static void AddCmd(this UpdateContext updateContext, MySqlCommand cmd)
        {
            if (updateContext.Cmds == null)
            {
                updateContext.Cmds = new List<MySqlCommand>();
            }

            updateContext.Cmds.Add(cmd);
        }

        public static void RollBack(this UpdateContext updateContext)
        {
            if (updateContext.Conns == null || updateContext.Conns.Any() == false)
            {
                return;
            }

            try
            {
                if (updateContext.Cmds != null)
                {
                    foreach (var item in updateContext.Cmds)
                    {
                        item.Dispose();
                    }

                    updateContext.Cmds.Clear();
                }
               
                foreach (var item in updateContext.Conns)
                {
                    var val = item.Value;
                    using (var conn = val.Item1)
                    {
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            continue;
                        }

                        var trann = val.Item2;
                        trann.Rollback();
                        trann.Dispose();
                        conn.Close();
                    }
                }
            }
            finally {
                updateContext.Conns.Clear();
            }
        }

        public static void Submit(this UpdateContext updateContext)
        {
            if (updateContext.Conns == null || updateContext.Conns.Any() == false)
            {
                return;
            }

            try
            {
                foreach (var item in updateContext.Conns)
                {
                    var val = item.Value;
                    var conn = val.Item1;
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        RollBack(updateContext);
                        return;
                    }

                    var trann = val.Item2;
                    trann.Commit();
                    trann.Dispose();
                    conn.Close();
                    conn.Dispose();
                }

                if (updateContext.Cmds != null)
                {
                    foreach (var item in updateContext.Cmds)
                    {
                        item.Dispose();
                    }

                    updateContext.Cmds.Clear();
                }
            }
            finally
            {
                updateContext.Conns.Clear();
            }
        }
    }
}
