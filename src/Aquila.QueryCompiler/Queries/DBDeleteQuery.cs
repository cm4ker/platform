using System;
using System.Text;
using Aquila.QueryBuilder.Interfaces;

namespace Aquila.QueryBuilder.Queries
{
    /// <summary>
    /// Represents DELETE instruction
    /// </summary>
    public class DBDeleteQuery : IDataChangeQuery
    {
        private DBFromClause _from;
        private DBWhereClause _where;

        private string _compileExpression;

        private string _deleteExpression;
        private string _fromExpression;
        private string _whereExpression;
        private DBTable _deleteTable;

        public DBDeleteQuery()
        {
            _from = new DBFromClause();
            _where = new DBWhereClause();
            _compileExpression = $"{{DeleteExpression}}\n{{FromExpression}}\n{{WhereExpression}}";
        }

        public DBTable DeleteTable
        {
            get { return _deleteTable; }
            set { _deleteTable = value; }
        }

        public void AddFrom(DBTable table)
        {
            _from.AddTable(table);
        }
        public void AddFrom(DBSubSelectQuery table)
        {
            _from.AddTable(table);
        }
        public DBJoinClause AddJoin(IDBTableDataSource table, JoinType type)
        {
            return _from.Join(table, type);
        }

        public DBLogicalOperation Where(DBClause clause1, CompareType type, DBClause clause2)
        {
            return _where.Where(clause1, type, clause2);
        }

        public DBParameterCollection Parameters => new DBParameterCollection(_where.Parameters);

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            StringBuilder ue = new StringBuilder();
            ue.AppendLine($"{SQLTokens.DELETE}");
            ue.Append($"\t[{DeleteTable.Alias}]");

            _deleteExpression = ue.ToString();

            if (_from.Tables.Count > 0)
                _fromExpression = _from.Compile();

            if (_where.LogicalClause.Operations.Count > 0)
                _whereExpression = _where.Compile();

            return $"{_deleteExpression}\n{_fromExpression}\n{_whereExpression}";
        }

        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }
    }
}