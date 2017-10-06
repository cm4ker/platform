using System;

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
            var newOp = new DBLogicalOperation(op.Owner, DBLogicalChainType.And, clause1, compareType, clause2);
            op.Owner.Operations.Add(newOp);
            return newOp;
        }

        public static DBLogicalOperation Or(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, DBLogicalChainType.Or, clause1, compareType, clause2);
            op.Owner.Operations.Add(newOp);

            return newOp;
        }

        public static DBLogicalOperation AndOr(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, op, DBLogicalChainType.Or, clause1, compareType, clause2);
            return newOp;
        }

        public static DBLogicalOperation AndAnd(this DBLogicalOperation op, DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(op.Owner, op, DBLogicalChainType.And, clause1, compareType, clause2);
            return newOp;
        }

        public static DBLogicalOperation And(this DBLogicalOperation op, DBClause clause1)
        {
            var newOp = new DBLogicalOperation(op.Owner, DBLogicalChainType.And, clause1);
            op.Owner.Operations.Add(newOp);
            return newOp;
        }

        public static DBLogicalOperation Or(this DBLogicalOperation op, DBClause clause1)
        {
            var newOp = new DBLogicalOperation(op.Owner, DBLogicalChainType.Or, clause1);
            op.Owner.Operations.Add(newOp);

            return newOp;
        }

        public static DBLogicalOperation AndOr(this DBLogicalOperation op, DBClause clause1)
        {
            var newOp = new DBLogicalOperation(op.Owner, op, DBLogicalChainType.Or, clause1);
            return newOp;
        }

        public static DBLogicalOperation AndAnd(this DBLogicalOperation op, DBClause clause1)
        {
            var newOp = new DBLogicalOperation(op.Owner, op, DBLogicalChainType.And, clause1);
            return newOp;
        }

        public static int RandomCharsInParams() => 12;
    }
}