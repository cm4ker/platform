using System.Text;

namespace QueryCompiler
{
    public class DBLogicalOperation : DBClause
    {
        private readonly DBLogicalClause _owner;
        private DBCompareOperation _operation;
        private DBLogicalOperation _chainedToken;
        private DBLogicalChainType _chainType;
        private DBParameterCollection _parameters;

        private DBLogicalOperation(DBLogicalClause owner, DBLogicalChainType chainType)
        {
            _parameters = new DBParameterCollection();
            _owner = owner;
            _chainType = chainType;
        }

        /// <summary>
        /// Only for is null opperation
        /// </summary>
        /// <param name="owner">owner construction</param>
        /// <param name="chainType"> and \ or </param>
        /// <param name="clause">field which be is null</param>
        public DBLogicalOperation(DBLogicalClause owner, DBLogicalChainType chainType, DBClause clause) : this(owner, chainType)
        {
            var compareOp = new DBCompareOperation();
            compareOp.AddIsNullClause(clause);

            if (clause is DBParameter) _parameters.Add(clause as DBParameter);
            _operation = compareOp;
        }

        public DBLogicalOperation(DBLogicalClause owner, DBLogicalChainType chainType, DBClause clause1, CompareType compareType, DBClause clause2)
            : this(owner, chainType)
        {
            var compareOp = new DBCompareOperation();
            compareOp.AddClause(clause1, compareType, clause2);

            if (clause1 is DBParameter) _parameters.Add(clause1 as DBParameter);
            if (clause2 is DBParameter) _parameters.Add(clause2 as DBParameter);

            _operation = compareOp;
        }

        public DBLogicalOperation(DBLogicalClause owner, DBLogicalOperation op, DBLogicalChainType type,
            DBClause clause1, CompareType compareType, DBClause clause2)
            : this(owner, type, clause1, compareType, clause2)
        {
            op._chainedToken = this;
            op._chainType = type;
        }

        public DBLogicalOperation(DBLogicalClause owner, DBLogicalOperation op, DBLogicalChainType type,
            DBClause clause)
            : this(owner, type, clause)
        {
            op._chainedToken = this;
            op._chainType = type;
        }

        public void CompareOperation(DBClause clause1, CompareType type, DBClause clause2)
        {
            var compareOp = new DBCompareOperation();
            compareOp.AddClause(clause1, type, clause2);
            _operation = compareOp;
        }

        public DBLogicalClause Owner => _owner;
        public DBParameterCollection Parameters => _parameters;

        public override string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            if (_chainedToken is null)
                if (_owner.Operations.IndexOf(this) > 0)
                    sb.AppendFormat(" {1} {0}", _operation.Compile(), _chainType.Compile());
                else
                    sb.Append(_operation.Compile());
            else
                sb.AppendFormat("({0} {1} {2})", _operation.Compile(), _chainType.Compile(), _chainedToken.Compile());
            return sb.ToString();
        }
    }
}