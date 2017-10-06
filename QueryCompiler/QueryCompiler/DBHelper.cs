using System;
using System.Data;
using System.Data.SqlClient;
using QueryCompiler.Schema;

namespace QueryCompiler
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

        public static DBFieldSchema GetUnknownSchema(this DBField field)
        {
            return new DBFieldSchema(SqlDbType.Variant, field.Name, 0, 0, 0, false, false, false, true);
        }

        //public static DBLogicalOperation And(this DBLogicalOperation op, DBClause clause1)
        //{
        //    var newOp = new DBLogicalOperation(op.Owner, DBLogicalChainType.And, clause1, );
        //    op.Owner.Operations.Add(newOp);
        //    return newOp;
        //}

        //public static DBLogicalOperation Or(this DBLogicalOperation op, DBClause clause1)
        //{
        //    var newOp = new DBLogicalOperation(op.Owner, DBLogicalChainType.Or, clause1);
        //    op.Owner.Operations.Add(newOp);

        //    return newOp;
        //}

        //public static DBLogicalOperation AndOr(this DBLogicalOperation op, DBClause clause1)
        //{
        //    var newOp = new DBLogicalOperation(op.Owner, op, DBLogicalChainType.Or, clause1);
        //    return newOp;
        //}

        //public static DBLogicalOperation AndAnd(this DBLogicalOperation op, DBClause clause1)
        //{
        //    var newOp = new DBLogicalOperation(op.Owner, op, DBLogicalChainType.And, clause1);
        //    return newOp;
        //}

        public static int RandomCharsInParams() => 12;


        /// <summary>
        /// Get the equivalent SQL data type of the given type.
        /// </summary>
        /// <param name="type">Type to get the SQL type equivalent of</param>
        public static SqlDbType GetSqlType(Type type)
        {
            if (type == typeof(string))
                return SqlDbType.NVarChar;

            if (type == typeof(byte[]))
                return SqlDbType.VarBinary;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            var param = new SqlParameter("", Activator.CreateInstance(type));
            return param.SqlDbType;
        }
    }
}