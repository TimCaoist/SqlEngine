using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class Column
    {
        [DataMember(Name = "db_name")]
        public string DBName { get; set; }

        [DataMember(Name = "table_name")]
        public string TableName { get; set; }

        [DataMember(Name = "name")]
        public string ColName { get; set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }

        [DataMember(Name = "data_type")]
        public string DataType { get; set; }

        [DataMember(Name="allow_null")]
        public string AllowNull { get; set; }

        [DataMember(Name = "default_value")]
        public string DefaultValue { set; get; }

        [DataMember(Name = "extra")]
        public string Extra { set; get; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        public bool IsKey {
            get {
                return !string.IsNullOrEmpty(Key) && Key.Equals("PRI", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
