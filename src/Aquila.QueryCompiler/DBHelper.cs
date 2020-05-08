using System;
using System.Collections.Generic;
using System.Data;
using Aquila.QueryBuilder.Schema;

namespace Aquila.QueryBuilder
{

    /// <summary>
    /// Just helper with some usefull functions
    /// </summary>
    public static class DBHelper
    {
        public static string GetRandomString(int length)
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, length);
        }

        public static string Compile(this DBLogicalChainType chainType)
        {
            switch (chainType)
            {
                case DBLogicalChainType.And: return SQLTokens.AND;
                case DBLogicalChainType.Or: return SQLTokens.OR;
                default: return "";
            }
        }

        public static DBLogicalOperation And(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, DBLogicalChainType.And, clause1, compareType, clause2, false);
            op.Owner.Operations.Add(newOp);
            return newOp;
        }

        public static DBLogicalOperation Or(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, DBLogicalChainType.Or, clause1, compareType, clause2, false);
            op.Owner.Operations.Add(newOp);

            return newOp;
        }

        public static DBLogicalOperation AndOr(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, op, DBLogicalChainType.Or, clause1, compareType, clause2, false);
            return newOp;
        }

        public static DBLogicalOperation AndAnd(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, op, DBLogicalChainType.And, clause1, compareType, clause2, false);

            return newOp;
        }

        public static DBLogicalOperation AndNot(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, DBLogicalChainType.And, clause1, compareType, clause2, true);
            op.Owner.Operations.Add(newOp);
            return newOp;
        }

        public static DBLogicalOperation OrNot(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, DBLogicalChainType.Or, clause1, compareType, clause2, true);
            op.Owner.Operations.Add(newOp);

            return newOp;
        }

        public static DBLogicalOperation AndOrNot(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, op, DBLogicalChainType.Or, clause1, compareType, clause2, true);
            return newOp;
        }

        public static DBLogicalOperation AndAndNot(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, op, DBLogicalChainType.And, clause1, compareType, clause2, true);
            return newOp;
        }

        public static DBFieldSchema GetUnknownSchema()
        {
            return new DBFieldSchema(DBType.Variant, 0, 0, 0, false, false, false, true);
        }

        public static int RandomCharsInParams() => 12;


        /// <summary>
        /// Get the equivalent SQL data type of the given type.
        /// </summary>
        /// <param name="type">Type to get the SQL type equivalent of</param>
        public static DBType GetDBType(Type type)
        {
            var typeMap = new Dictionary<Type, DBType>();
            typeMap[typeof(byte)] = DBType.Binary;
            typeMap[typeof(sbyte)] = DBType.Binary;
            typeMap[typeof(short)] = DBType.SmallInt;
            typeMap[typeof(ushort)] = DBType.SmallInt;
            typeMap[typeof(int)] = DBType.Int;
            typeMap[typeof(uint)] = DBType.Int;
            typeMap[typeof(long)] = DBType.BigInt;
            typeMap[typeof(ulong)] = DBType.BigInt;
            typeMap[typeof(float)] = DBType.Real;
            typeMap[typeof(double)] = DBType.Decimal;
            typeMap[typeof(decimal)] = DBType.Decimal;
            typeMap[typeof(bool)] = DBType.Bit;
            typeMap[typeof(string)] = DBType.VarChar;
            typeMap[typeof(char)] = DBType.VarChar;
            typeMap[typeof(Guid)] = DBType.UniqueIdentifier;
            typeMap[typeof(DateTime)] = DBType.DateTime;
            typeMap[typeof(DateTimeOffset)] = DBType.DateTimeOffset;
            typeMap[typeof(byte[])] = DBType.Binary;
            typeMap[typeof(byte?)] = DBType.VarBinary;
            typeMap[typeof(sbyte?)] = DBType.VarBinary;
            typeMap[typeof(short?)] = DBType.SmallInt;
            typeMap[typeof(ushort?)] = DBType.SmallInt;
            typeMap[typeof(int?)] = DBType.Int;
            typeMap[typeof(uint?)] = DBType.Int;
            typeMap[typeof(long?)] = DBType.BigInt;
            typeMap[typeof(ulong?)] = DBType.BigInt;
            typeMap[typeof(float?)] = DBType.Real;
            typeMap[typeof(double?)] = DBType.Decimal;
            typeMap[typeof(decimal?)] = DBType.Decimal;
            typeMap[typeof(bool?)] = DBType.Bit;
            typeMap[typeof(char?)] = DBType.VarChar;
            typeMap[typeof(Guid?)] = DBType.UniqueIdentifier;
            typeMap[typeof(DateTime?)] = DBType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DBType.DateTimeOffset;

            return typeMap[type];
        }
        
        public static string Compile(this DBType type)
        {
            var typeMap = new Dictionary<DBType, string>();
            typeMap[DBType.BigInt] = "bigint";
            typeMap[DBType.Binary] = "binary";
            typeMap[DBType.Bit] = "bit";
            typeMap[DBType.Char] = "char";
            typeMap[DBType.DateTime] = "datetime";
            typeMap[DBType.Decimal] = "decimal";
            typeMap[DBType.Float] = "float";
            typeMap[DBType.Image] = "image";
            typeMap[DBType.Int] = "int";
            typeMap[DBType.Money] = "money";
            typeMap[DBType.NChar] = "nchar";
            typeMap[DBType.NText] = "ntext";
            typeMap[DBType.NVarChar] = "nvarchar";
            typeMap[DBType.Real] = "real";
            typeMap[DBType.UniqueIdentifier] = "uniqueidentifier";
            typeMap[DBType.SmallDateTime] = "smalldatetime";
            typeMap[DBType.SmallInt] = "smallint";
            typeMap[DBType.SmallMoney] = "smallmoney";
            typeMap[DBType.Timestamp] = "Timestamp";
            typeMap[DBType.TinyInt] = "tinyint";
            typeMap[DBType.VarBinary] = "varbinary";
            typeMap[DBType.VarChar] = "varchar";
            typeMap[DBType.Variant] = "variant";
            typeMap[DBType.Xml] = "xml";
            typeMap[DBType.Udt] = "udt";
            typeMap[DBType.Structured] = "structured";
            typeMap[DBType.Date] = "Date";
            typeMap[DBType.Time] = "Time";
            typeMap[DBType.DateTime2] = "datetime2";
            typeMap[DBType.DateTimeOffset] = "datetimeoffcet";

            return typeMap[type];
        }
    }
}