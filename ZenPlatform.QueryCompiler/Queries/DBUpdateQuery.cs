using System;
using System.Data.Common;
using System.Text;
using ZenPlatform.QueryBuilder.Interfaces;

namespace ZenPlatform.QueryBuilder.Queries
{
    public class DBUpdateQuery : IDataChangeQuery
    {
        private DBFromClause _from;
        private DBWhereClause _where;
        private DBSetUpdateClause _setUpdate;
        private DBParameterCollection _parameters;

        private string _compileExpression;

        private string _updateExpression;
        private string _fromExpression;
        private string _whereExpression;
        private DBTable _updateTable;



        public DBUpdateQuery()
        {
            _from = new DBFromClause();
            _where = new DBWhereClause();
            _setUpdate = new DBSetUpdateClause();
            _parameters = new DBParameterCollection();

            _setUpdate.Parameters.OnChange += Parameters_OnChange;
            _where.Parameters.OnChange += Parameters_OnChange;

            _compileExpression = $"{{UpdateExpression}}\n{{FromExpression}}\n{{WhereExpression}}";
        }

        private void Parameters_OnChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (DBParameter item in e.NewItems)
                {
                    _parameters.Add(item);
                }
            if (e.OldItems != null)
                foreach (DBParameter item in e.OldItems)
                {
                    _parameters.Remove(item);
                }
        }

        public DBTable UpdateTable
        {
            get { return _updateTable; }
            set { _updateTable = value; }
        }

        public DBParameterCollection Parameters => _parameters;//new DBParameterCollection(_setUpdate.Parameters.Union(_where.Parameters));

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

        public void AddField(DBTableField field)
        {
            if (_updateTable != null && field.Owner as DBTable != _updateTable)
            {
                throw new Exception("Trying to update fields in differend tables");
            }
            if (_updateTable is null)
                _updateTable = field.Owner as DBTable;

            _setUpdate.AddField(field);
        }

        public DBLogicalOperation Where(DBClause clause1, CompareType type, DBClause clause2)
        {
            return _where.Where(clause1, type, clause2);
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            StringBuilder ue = new StringBuilder();
            ue.AppendLine($"{SQLTokens.UPDATE}");
            ue.AppendLine($"\t[{UpdateTable.Alias}]");

            ue.AppendLine(_setUpdate.Compile().Replace("\n", "\n\t"));

            _updateExpression = ue.ToString();

            if (_from.Tables.Count > 0)
                _fromExpression = _from.Compile();

            if (_where.LogicalClause.Operations.Count > 0)
                _whereExpression = _where.Compile();

            return $"{_updateExpression}\n{_fromExpression}\n{_whereExpression}";
        }

        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }
    }

    public interface IQueryable : IDBToken
    {

    }

    public interface IParametrized
    {
        DBParameterCollection Parameters { get; }
    }

    public interface IDataChangeQuery : IParametrizedQuery
    {

    }

    public interface IDataReturnQuery : IParametrizedQuery
    {

    }

    public interface IParametrizedQuery : IQueryable, IParametrized
    {

    }
}