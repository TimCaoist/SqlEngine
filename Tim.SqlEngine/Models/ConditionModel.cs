using System.Runtime.Serialization;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class ConditionModel
    {
        [DataMember(Name = "eval")]
        public string Eval { get;  set; }

        [DataMember(Name = "query_type")]
        public int QueryType { get; set; }

        [DataMember(Name = "stop_by_eval")]
        public bool StopByEval { get; set; }

        [DataMember(Name = "result_eval")]
        public string ResultEval { get; set; }
    }
}
