using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.SqlHelper;

namespace Tim.SqlEngine.Models
{
    public class UpdateContext : BaseContext<UpdateContext, UpdateHandlerConfig, UpdateConfig>, IContext
    {

        private IDictionary<string, Tuple<MySqlConnection, MySqlTransaction>> _conns;

        public IDictionary<string, Tuple<MySqlConnection, MySqlTransaction>> Conns
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.Conns;
                }

                return _conns;
            }
            set
            {
                if (Parent != null)
                {
                    Parent.Conns = value;
                    return ;
                }

                _conns = value;
            }
        }

        private bool _excuteFail;

        public bool ExcuteFail {
            get
            {
                if (Parent != null)
                {
                    return Parent.ExcuteFail;
                }

                return _excuteFail;
            }
            set
            {
                if (Parent != null)
                {
                    Parent.ExcuteFail = value;
                    return;
                }

                _excuteFail = value;
            }
        }

        ~UpdateContext() {
            TranHelper.RollBack(this);
        }
    }
}
