using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Linq;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.Metadata;
using Aquila.Runtime;
using Aquila.Runtime.Querying;
using Microsoft.IdentityModel.Protocols;

namespace Aquila.Core
{
    public class AqParamValue
    {
        public AqParamValue(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }
    }

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
            if (value != null)
                param.DbType = typeMap[value.GetType()];

            param.Value = value ?? DBNull.Value;

            command.Parameters.Add(param);
        }

        public static void InvokeInsert(AqContext context, string mdName, AqParamValue[] parameters)
        {
            using var cmd = PrepareCore(context, parameters, mdName, (SMEntity semantic, out QLangElement element) =>
                CRUDQueryGenerator.CompileInsert(semantic, context, out element));
            cmd.ExecuteNonQuery();
        }

        public static void InvokeUpdate(AqContext context, string mdName, AqParamValue[] parameters)
        {
            using var cmd = PrepareCore(context, parameters, mdName, (SMEntity semantic, out QLangElement element) =>
                CRUDQueryGenerator.CompileUpdate(semantic, context, out element));
            cmd.ExecuteNonQuery();
        }

        public static void InvokeDelete(AqContext context, string mdName, AqParamValue[] parameters)
        {
            using var cmd = PrepareCore(context, parameters, mdName, (SMEntity semantic, out QLangElement element) =>
                CRUDQueryGenerator.CompileDelete(semantic, context, out element));
            cmd.ExecuteNonQuery();
        }

        public static object InvokeSelect(AqContext context, string mdName, AqParamValue[] parameters,
            AqReadDelegate readAction)
        {
            var needCheckSec = false;

            using var cmd = PrepareCore(context, parameters, mdName,
                (SMEntity semantic, out QLangElement element) =>
                {
                    return CRUDQueryGenerator.CompileSelect(semantic, context, out element);
                });

            var dataReader = cmd.ExecuteReader();
            object result = null;

            while (dataReader.Read())
            {
                if (needCheckSec)
                {
                    //Last field is security field
                    var secRes = dataReader.GetInt32(dataReader.FieldCount - 1);

                    if (secRes == 0)
                    {
                        //TODO: throw security exception
                    }
                }

                result = readAction(dataReader);
            }

            dataReader.Close();
            dataReader.Dispose();

            //TODO: throw not found exception if result is null
            return result;
        }

        private delegate string CompileQueryDelegate(SMEntity semantic, out QLangElement model);

        private static DbCommand PrepareCore(AqContext context, AqParamValue[] parameters, string mdName,
            CompileQueryDelegate action)
        {
            var semantic = context.MetadataProvider.GetSemanticByName(mdName);
            var commandText = action(semantic, out QLangElement model);

            var result = model.Find<QParameterBase>().DistinctBy(x => x.Name)
                .Select(x => new { QParam = x, QName = x.Name, RName = x.GetDbName() });

            DbCommand cmd = context.CreateCommand();
            cmd.CommandText = commandText;
            foreach (var param in result)
            {
                var types = param.QParam.GetExpressionType().ToImmutableArray();
                if (types.Length > 1)
                {
                    //Complex type
                    var schema = DRContextHelper.GetPropertySchemas(param.QName, types);

                    foreach (var sh in schema)
                    {
                        var dbParam = cmd.CreateParameter();
                        dbParam.ParameterName = sh.Prefix + param.RName + sh.Postfix;

                        var pValue = parameters.FirstOrDefault(x => x.Name == sh.FullName);

                        if (pValue != null)
                        {
                            dbParam.DbType = typeMap[pValue.Value.GetType()];
                            dbParam.Value = pValue.Value;
                        }

                        cmd.Parameters.Add(dbParam);
                    }
                }
                else
                {
                    var dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = param.RName;

                    //try to find passed real params
                    var pValue = parameters.FirstOrDefault(x => x.Name == param.QName);

                    if (pValue != null)
                    {
                        dbParam.DbType = typeMap[pValue.Value.GetType()];
                        dbParam.Value = pValue.Value;
                    }

                    cmd.Parameters.Add(dbParam);
                }

                //TODO: try to find parameter in global parameters
            }

            return cmd;
        }
    }

    public delegate object AqReadDelegate(DbDataReader reader);
}