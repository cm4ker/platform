using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace QueryCompiler
{
    public class DBUpdateQuery : IDataChangeQuery
    {
        private DBFromClause _from;
        private DBWhereClause _where;
        private DBSetUpdateClause _setUpdate;

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
            _compileExpression = $"{{UpdateExpression}}\n{{FromExpression}}\n{{WhereExpression}}";
        }

        public DBTable UpdateTable
        {
            get { return _updateTable; }
            //set { _updateTable = value; }
        }

        public List<DBParameter> Parameters => _setUpdate.Parameters.Union(_where.Parameters).ToList();

        public void AddFrom(DBTable table)
        {
            _from.AddTable(table);
        }
        public void AddFrom(DBSubSelectQuery table)
        {
            _from.AddTable(table);
        }
        public DBJoinClause AddJoin(IDBTableDataSorce table, JoinType type)
        {
            return _from.Join(table, type);
        }

        public void AddField(DBTableField field)
        {
            if (_updateTable != null && field.Owner as DBTable != _updateTable)
            {
                throw new Exception("Trying to update fields in differend tables");
            }
            _updateTable = field.Owner as DBTable;
            _setUpdate.AddField(field);
        }

        //public void AddWhere(DBClause clause1, CompareType type, DBClause clause2)
        //{
        //    _where.AddClause(clause1, type, clause2);
        //}
        //public void AddAndWhere(DBClause clause1, CompareType type, DBClause clause2)
        //{
        //    _where.AndClause(clause1, type, clause2);
        //}
        //public void AddOrWhere(DBClause clause1, CompareType type, DBClause clause2)
        //{
        //    _where.OrClause(clause1, type, clause2);
        //}

        public void Where(DBClause clause1, CompareType type, DBClause clause2)
        {
            _where.Where(clause1, type, clause2);
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

            return StandartCompilers.SimpleCompiler(_compileExpression,
                new
                {
                    UpdateExpression = _updateExpression,
                    FromExpression = _fromExpression,
                    WhereExpression = _whereExpression,
                });
        }

        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }
    }

    public interface IQueryable : IToken
    {

    }

    public interface IDataChangeQuery : IQueryable
    {
        List<DBParameter> Parameters { get; }
    }
}