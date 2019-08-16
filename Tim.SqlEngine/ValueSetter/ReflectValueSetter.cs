﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;

namespace Tim.SqlEngine.ValueSetter
{
    public class ReflectValueSetter : IValueSetter
    {
        private readonly string AssemblyString;

        private readonly string TypeStr;

        private object instance;

        private Type intanceType;

        public ReflectValueSetter(string assemblyString, string typeStr)
        {
            AssemblyString = assemblyString;
            TypeStr = typeStr;
        }

        public object CreateInstance()
        {
            instance = ReflectUtil.ReflectUtil.CreateInstance(AssemblyString, TypeStr);
            intanceType = instance.GetType();
            return instance;
        }

        public IEnumerable<object> SetterDatas(QueryConfig queryConfig, MySqlDataReader dataReader, IEnumerable<string> columns)
        {
            ICollection<object> datas = new List<object>();
            var alias = queryConfig.Alais;
            var count = columns.Count();
            var alaisParser = new AlaisParser(columns, alias);

            while (dataReader.Read())
            {
                object data = ReflectUtil.ReflectUtil.CreateInstance(AssemblyString, TypeStr);
                var type = data.GetType();
                for (var i = 0; i < count; i++)
                {
                    var col = columns.ElementAt(i);
                    var realName = alaisParser.GetName(i, col);
                    var property = type.GetProperty(realName);
                    if (property == null)
                    {
                        continue;
                    }

                    property.SetValue(data, dataReader[col]);
                }

                datas.Add(data);

                if (queryConfig.OnlyOne) {
                    break;
                }
            }

            return datas;
        }

        public void SetterField(string filed, object data)
        {
            var property = intanceType.GetProperty(filed);
            if (property == null)
            {
                return;
            }

            if (property.PropertyType != data.GetType())
            {
                throw new ArgumentException(string.Concat(filed, "字段类型不匹配！"));
            }

            property.SetValue(instance, data);
        }
    }
}
