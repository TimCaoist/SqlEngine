using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public abstract class BaseContext<TContext, TConfig, TQueryConfig> where TContext: BaseContext<TContext, TConfig, TQueryConfig>, IContext where TQueryConfig:BaseHadlerConfig where TConfig:BaseHadlerConfig, IHandlerConfig
    {
        public BaseContext() : this(null)
        {
        }

        public BaseContext(TContext parent)
        {
            Parent = parent;
            Childs = new List<TContext>();
        }

        public TContext Parent { get; set; }

        public ICollection<TContext> Childs { get; set; }

        public object Data { get; set; }

        private TConfig _handlerConfig;

        public TConfig HandlerConfig
        {
            get
            {
                if (Parent == null)
                {
                    return _handlerConfig;
                }

                return Parent.HandlerConfig;
            }
            set
            {
                _handlerConfig = value;
            }
        }

        public IEnumerable<TQueryConfig> Configs { get; set; }

        public TQueryConfig Config
        {
            get
            {
                if (Configs == null || Configs.Any() == false)
                {
                    throw new System.ArgumentNullException("queryConfig");
                }

                return Configs.First();
            }
        }


        public IDictionary<string, object> _params;

        /// <summary>
        /// 整个上下文共用参数
        /// </summary>
        public IDictionary<string, object> Params
        {
            get
            {
                if (Parent == null || _params != null)
                {
                    return _params;
                }

                return Parent._params;
            }
            set
            {
                _params = value;
            }
        }

        /// <summary>
        /// 当前执行上下文参数
        /// </summary>
        public IDictionary<string, object> ContentParams
        {
            get; set;
        } = new Dictionary<string, object>();

        public object _complexData;

        public object ComplexData
        {
            get {
                if (_complexData != null)
                {
                    return _complexData;
                }

                if (Parent == null)
                {
                    return null;
                }

                return Parent.ComplexData;
            }
            set {
                _complexData = value;
            }
        }

        public BaseHadlerConfig GetConfig() {
            return Config;
        }

        public IHandlerConfig GetHandlerConfig()
        {
            return HandlerConfig;
        }

        public IContext GetParent()
        {
            return Parent;
        }
    }
}
