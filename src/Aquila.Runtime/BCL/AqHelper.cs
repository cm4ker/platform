using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Aquila.Core
{
    public static class AqHelper
    {
        private static Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>()
        {
            {
                typeof(byte), DbType.Byte
            },
            {
                typeof(sbyte), DbType.SByte
            },
            {
                typeof(short), DbType.Int16
            },
            {
                typeof(ushort), DbType.UInt16
            },
            {
                typeof(int), DbType.Int32
            },
            {
                typeof(uint), DbType.UInt32
            },
            {
                typeof(long), DbType.Int64
            },
            {
                typeof(ulong), DbType.UInt64
            },
            {
                typeof(float), DbType.Single
            },
            {
                typeof(double), DbType.Double
            },
            {
                typeof(decimal), DbType.Decimal
            },
            {
                typeof(bool), DbType.Boolean
            },
            {
                typeof(string), DbType.String
            },
            {
                typeof(char), DbType.StringFixedLength
            },
            {
                typeof(Guid), DbType.Guid
            },
            {
                typeof(DateTime), DbType.DateTime
            },
            {
                typeof(byte[]), DbType.Binary
            }
        };

        public static void AddParameter(DbCommand command, string name, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = typeMap[value.GetType()];
            param.Value = value;
            command.Parameters.Add(param);
        }
    }
}