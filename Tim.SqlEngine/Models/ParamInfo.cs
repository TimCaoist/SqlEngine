﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public class ParamInfo
    {
        private object _originalData;

        public object OriginalData  
        {
            get{
                return _originalData ?? Data;
            }
            set {
                _originalData = value;
            }
         }

        public ParamType Type { get; set; }

        public string Name { get; set; }

        public object Data { get; set; }

        public Match Match { get; internal set; }

        public bool ReBuilder { get; set; }
    }
}
