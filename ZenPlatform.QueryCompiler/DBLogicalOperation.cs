using System.Text;

namespace ZenPlatform.QueryCompiler
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
            _parameters.OnChange += _parameters_OnChange;
            _owner = owner;
            _chainType = chainType;

        }

        private void _parameters_OnChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_owner != null)
            {
                if (e.NewItems != null)
                    foreach (var item in e.NewItems)
                    {
                        _owner.Parameters.Add(item as DBParameter, true);
                    }

                if (e.OldItems != null)
                    foreach (var item in e.OldItems)
                    {
                        _owner.Parameters.Remove(item as DBParameter);
                    }
            }

        }

        public DBLogicalOperation(DBLogicalClause owner, DBLogicalChainType chainType, DBClause clause1, CompareType compareType, DBClause clause2, bool negotiation)
            : this(owner, chainType)
        {
            var compareOp = new DBCompareOperation();
            if (compareType == CompareType.IsNull)
                compareOp.AddIsNullClause(clause1, negotiation);
            else
                compareOp.AddClause(clause1, compareType, clause2, negotiation);

            if (clause1 is DBParameter) _parameters.Add(clause1 as DBParameter, true);
            if (clause2 is DBParameter) _parameters.Add(clause2 as DBParameter, true);

            _operation = compareOp;
        }

        public DBLogicalOperation(DBLogicalClause owner, DBLogicalOperation op, DBLogicalChainType type,
            DBClause clause1, CompareType compareType, DBClause clause2, bool negotiation)
            : this(owner, type, clause1, compareType, clause2, negotiation)
        {

            var chainedOp = op;
            while (chainedOp._chainedToken != null)
            {
                chainedOp = chainedOp._chainedToken;
            }

            chainedOp._chainedToken = this;

            op._chainType = type;
        }

        //public DBLogicalOperation(DBLogicalClause owner, DBLogicalOperation op, DBLogicalChainType type,
        //    DBClause clause)
        //    : this(owner, type, clause)
        //{
        //    op._chainedToken = this;
        //    op._chainType = type;
        //}

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