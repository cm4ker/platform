using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder
{
    public class SQLTypeBuilder
    {
        public ColumnTypeBinary Binary(int size)
        {
            return new ColumnTypeBinary() { Size = size };
        }

        public ColumnTypeVarBinary VarBinary(int size)
        {
            return new ColumnTypeVarBinary() { Size = size };
        }

        public ColumnTypeGuid Guid()
        {
            return new ColumnTypeGuid();
        }

        public ColumnTypeInt Int()
        {
            return new ColumnTypeInt();
        }

        public ColumnTypeText Text(int size)
        {
            return new ColumnTypeText { Size = size };
        }

        public ColumnTypeVarChar Varchar(int size)
        {
            return new ColumnTypeVarChar { Size = size };
        }


        public ColumnTypeNumeric Numeric(int scale, int precision)
        {
            return new ColumnTypeNumeric { Scale = scale, Precision = precision };
        }

        public ColumnTypeDataTime Date()
        {
            return new ColumnTypeDataTime();
        }

        public ColumnTypeBool Bool()
        {
            return new ColumnTypeBool();
        }

        public ColumnType Parse(string typeName)
        {
            Regex r = new Regex(@"(\b[^()]+)(\((.*)\))?$");

            var match = r.Match(typeName);
            string[] args = null;

            if (match.Success)
            {
                if (match.Groups.Count == 1)
                {
                    typeName = match.Groups[1].Value;
                }
                else
                {
                    typeName = match.Groups[1].Value;
                    args = match.Groups[3].Value.Split(",");
                }
            }

            return typeName switch
            {
                "text" => Text(int.Parse(args[0])),
                "int" => Int(),
                "guid" => Guid(),
                "binary" => Binary(int.Parse(args[0])),
                "varbinary" => VarBinary(int.Parse(args[0])),
                "varchar" => Varchar(int.Parse(args[0])),
                "numeric" => Numeric(int.Parse(args[0]), int.Parse(args[1])),
                "bool" => Bool(),
                "datetime" => Date(),
                _ => throw new Exception($"Type def: {typeName} can't be resolved")
            };
        }
    }
}