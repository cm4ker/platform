using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace QueryCompiler
{
    /// <summary>
    /// Represents complitely logical operation which returns true or false
    /// this operation consists from 1+ units (DBLogicalOperaion)
    /// </summary>
    public class DBLogicalClause : DBClause
    {
        private DBParameterCollection _parameters;

        public DBLogicalClause()
        {
            var ops = new ObservableCollection<DBLogicalOperation>();
            ops.CollectionChanged += Ops_CollectionChanged;
            Operations = ops;
            _parameters = new DBParameterCollection();
        }

        private void Ops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DBLogicalOperation op in e.NewItems)
                {
                    op.Parameters.OnChange += Parameters_OnChange;
                    _parameters.AddRange(op.Parameters, true);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DBLogicalOperation op in e.OldItems)
                {
                    op.Parameters.OnChange -= Parameters_OnChange;
                    foreach (var item in op.Parameters)
                    {
                        _parameters.Remove(item);
                    }
                }
            }
        }

        private void Parameters_OnChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                _parameters.AddRange(e.NewItems as IList<DBParameter>, true);
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DBParameter item in e.NewItems)
                {
                    _parameters.Remove(item);
                }

            }
        }

        public IList<DBLogicalOperation> Operations { get; set; }

        public DBParameterCollection Parameters
        {
            get
            {
                return _parameters;
                //var ps = new DBParameterCollection();
                //foreach (var op in Operations)
                //{
                //    ps.AddRange(op.Parameters, true);
                //}
                //return ps;
            }
        }

        public override string Compile(bool recompile = false)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var operation in Operations)
            {
                sb.Append(operation.Compile());
            }
            return sb.ToString();
        }

        public DBLogicalOperation Where(DBClause clause1, CompareType compareType, DBClause clause2)
        {
            var newOp = new DBLogicalOperation(this, DBLogicalChainType.And, clause1, compareType, clause2);
            Operations.Add(newOp);
            return newOp;
        }

        public DBLogicalOperation Where(DBClause clause1)
        {
            var newOp = new DBLogicalOperation(this, DBLogicalChainType.And, clause1);
            Operations.Add(newOp);
            return newOp;
        }

    }
}