using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public class Context
    {
        public Context() : this(null)
        {
        }

        public Context(Context parent)
        {
            this.Parent = parent;
            this.Childs = new List<Context>();
        }

        public Context Parent { get; }

        public object Data { get; set; }

        public ICollection<Context> Childs { get; set; } 

        private HandlerConfig _handlerConfig;

        public HandlerConfig HandlerConfig {
            get {
                if (Parent == null)
                {
                    return _handlerConfig;
                }

                return Parent.HandlerConfig;
            }
            set {
                _handlerConfig = value;
            }
        }

        public IEnumerable<QueryConfig> Configs { get; set; }

        public QueryConfig Config {
            get {
                if (Configs == null || Configs.Any() == false)
                {
                    throw new System.ArgumentNullException("queryConfig");
                }

                return Configs.First();
            }
        }

        public IDictionary<string, object> _queryParams;

        public IDictionary<string, object> QueryParams {
            get
            {
                if (_queryParams == null)
                {
                    return Parent.QueryParams;
                }

                return _queryParams;
            }
            set
            {
                _queryParams = value;
            }
        }

        public IDictionary<string, object> ContentQueryParams
        {
            get;set;
        } = new Dictionary<string, object>();
    }
}
