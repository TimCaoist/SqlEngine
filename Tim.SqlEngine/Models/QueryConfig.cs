using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class QueryConfig : BaseHadlerConfig
    {

        [DataMember(Name = "one")]
        public bool OnlyOne { get; set; }

        [DataMember(Name = "field")]
        public string Filed { get;  set; }

        [DataMember(Name = "alais")]
        public string[] Alais { get; set; }
    }
}
