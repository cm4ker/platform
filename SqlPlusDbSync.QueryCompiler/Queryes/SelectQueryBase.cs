using System;
using System.Collections.Generic;
using System.Text;
using SqlPlusDbSync.QueryCompiler.Queryes.Operator;

namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public abstract class SelectQueryBase : IQueryObject
    {
        private ITable _table;
        private readonly List<Realtion> _relations;
        private readonly List<ISelectItem> _selectItems;
        private readonly List<IGroupItem> _groupItems;

        protected SelectQueryBase()
        {
            _relations = new List<Realtion>();
            _selectItems = new List<ISelectItem>();
            _groupItems = new List<IGroupItem>();

        }

        public ITable Table
        {
            get { return _table; }
            set { _table = value; }
        }

        public List<Realtion> RelationTables
        {
            get
            {
                return _relations;
            }
        }

        public List<ISelectItem> SelectItems
        {
            get
            {
                return _selectItems;
            }
        }

        public IBooleanResultOperator WhereOperator { get; set; }

        public List<IGroupItem> GroupItems
        {
            get { return _groupItems; }
        }

        public IBooleanResultOperator HavingOperator { get; set; }

        protected virtual void AddSelect(ISelectItem element)
        {
            _selectItems.Add(element);
        }

        public virtual void AddRelationTable(ITable relationTable, RelationalOptions oprions)
        {
            _relations.Add(new Realtion { Table = relationTable, Options = oprions });

        }

        public virtual void AddGroup(IGroupItem group)
        {
            _groupItems.Add(group);
        }

        public virtual string CompileSelectStatement()
        {
            if (_selectItems.Count == 0)
                throw new Exception("Please select any field or expression or query");
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT");
            foreach (var selectItem in _selectItems)
            {
                var compiled = selectItem.Compile();
                if (_selectItems.IndexOf(selectItem) > 0)
                    sb.Append(",");
                if (string.IsNullOrEmpty(selectItem.Alias))
                    sb.AppendFormat("\t{0}", selectItem.Compile());
                else
                    sb.AppendFormat("\t[{0}] = {1}", selectItem.Alias, selectItem.Compile());
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public virtual string CompileFromStatement()
        {
            if (_selectItems.Count == 0)
                throw new Exception("Please point to any table or query object");
            StringBuilder sb = new StringBuilder();
            var co = new CompileOptions() { IsSubQuery = true };

            sb.AppendLine("FROM");
            if (_table is SelectQuery)
                sb.AppendFormat("\t{0}", _table.Compile(co));
            else
                sb.AppendFormat("\t{0}", _table.Compile());
            sb.AppendLine();

            foreach (var rel in _relations)
            {
                sb.AppendFormat("\t{0} JOIN {1} ON {2}", rel.Options.GetJoinType(), rel.Table.Compile(co), rel.Options.Condition.Compile());
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public virtual string CompileWhereStatement()
        {
            if (WhereOperator is null) return "";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("WHERE");
            sb.AppendFormat("\t{0}", WhereOperator.Compile());
            sb.AppendLine();
            return sb.ToString();
        }

        public virtual string CompileGroupStatement()
        {
            if (_groupItems.Count == 0) return "";
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("GROUP BY");
            foreach (var groupItem in _groupItems)
            {
                var compiled = groupItem.Compile();
                if (_groupItems.IndexOf(groupItem) > 0)
                    sb.Append(",");
                sb.AppendFormat("\t{0}", compiled);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public virtual string CompileHavingStatement()
        {
            if (HavingOperator is null) return "";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HAVING");
            sb.AppendFormat("\t{0}", HavingOperator.Compile());
            return sb.ToString();
        }

        public virtual string Compile()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(CompileSelectStatement());
            sb.Append(CompileFromStatement());
            sb.Append(CompileWhereStatement());
            sb.Append(CompileGroupStatement());
            sb.Append(CompileHavingStatement());

            return sb.ToString();
        }

        public virtual string Compile(CompileOptions options)
        {
            return Compile();
        }
    }
}