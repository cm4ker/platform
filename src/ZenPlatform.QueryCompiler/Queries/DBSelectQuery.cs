using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using ZenPlatform.QueryBuilder.Interfaces;

namespace ZenPlatform.QueryBuilder.Queries
{
    public class DBSelectQuery : IDBTablesContainer, IDBFieldContainer, IDataReturnQuery
    {
        private string _compileExpression;

        private string _intoTable;
        private uint _topCount;

        private List<DBField> _fields;

        private DBFromClause _from;
        private DBWhereClause _where;
        private DBOrderByClause _orderby;
        private DBGroupByClause _groupby;


        private string _compiled;

        public DBSelectQuery()
        {
            _fields = new List<DBField>();
            _from = new DBFromClause();
            _where = new DBWhereClause();
            _orderby = new DBOrderByClause();
            _groupby = new DBGroupByClause();
        }


        public DBSelectQuery(string tableName): this()
        {
            DBTable dbTable = new DBTable(tableName);
            _from.AddTable(dbTable);
        }

        public DBSelectQuery Select(string fieldName)
        {
            _fields.Add(new DBSelectField(_from.RootTable, fieldName));
            return this;
        }

        public DBSelectQuery Select(params string[] fieldsName)
        {
            foreach (var fieldName in fieldsName)
            {
                Select(fieldName);
            }

            return this;

        }

        public DBSelectQuery Select(DBField clause)
        {

            _fields.Add(clause);
            return this;
        }

        public void SelectAllFieldsFromSourceTables(string prefixAll = "")
        {
            foreach (var table in _from.Tables)
            {
                foreach (var field in table.Fields)
                {
                    var dbField = field.Clone() as DBField;

                    var selectField = dbField.ToSelectField();
                    if (!string.IsNullOrEmpty(prefixAll))
                        selectField.SetAliase($"{prefixAll}{selectField.Name}");
                    _fields.Add(selectField);
                }
            }
        }

        public DBSelectQuery Top(uint rowsCount)
        {
            _topCount = rowsCount;
            return this;
        }

        public DBSelectQuery From(String tableName)
        {
            From(new DBTable(tableName));
            return this;
        }

        public DBSelectQuery From(DBTable table)
        {
            _from.AddTable(table);
            return this;
        }

        public DBSelectQuery From(DBSelectQuery subquery, string alias)
        {
            _from.AddTable(new DBSubSelectQuery(subquery, alias));
            return this;
        }

        public DBSelectQuery From(DBSubSelectQuery subquery)
        {
            _from.AddTable(subquery);
            return this;
        }

        public DBSelectQuery Into(string intoTableName)
        {
            _intoTable = intoTableName;
            return this;
        }

        public DBOrderByClause OrderBy(DBClause clause)
        {
            _orderby.ThenOrderBy(clause);
            return _orderby;
        }

        public DBOrderByClause OrderByDesc(DBClause clause)
        {
            _orderby.ThenOrderByDesc(clause);
            return _orderby;
        }

        public DBGroupByClause GroupBy(DBClause clause)
        {

            _groupby.ThenGroupBy(clause);
            return _groupby;
        }

        public  void Where(string fieldName1, CompareType type, string fieldName2)
        {
           // _where.Where(new DBSelectField(), type, new DBSelectField());
        }


        public DBLogicalOperation Where(DBClause clause1, CompareType type, DBClause clause2)
        {

            return _where.Where(clause1, type, clause2);
        }

        public DBLogicalOperation WhereNot(DBClause clause1, CompareType type, DBClause clause2)
        {

            return _where.WhereNot(clause1, type, clause2);
        }

        public void ClearWhere()
        {
            _where.Clear();
        }

        public DBParameterCollection Parameters
        {
            get
            {
                var par = _where.Parameters.Union(_from.GetParameters());
                return new DBParameterCollection(par);
            }
        }

        public DBJoinClause Join(IDBTableDataSource table, JoinType joinType = JoinType.Inner)
        {
            return _from.Join(table, joinType);
        }

        public List<DBField> Fields
        {
            get { return _fields; }
        }

        public DBSubSelectQuery GetAsSubQuery(string aliase = "")
        {
            return new DBSubSelectQuery(this, aliase);
        }

        public object Clone()
        {
            if (this.MemberwiseClone() is DBSelectQuery clone)
            {
                clone._where = _where.Clone() as DBWhereClause;
                clone._fields = _fields.ToList();
                clone._from = _from.Clone() as DBFromClause;
                return clone;
            }

            throw new Exception("Can't clone this object");

        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(_compiled) && !recompile)
                return _compiled;

            if (_fields is null)
                throw new Exception();

            sb.Append(SQLTokens.SELECT);
            if (_topCount != 0)
            {
                sb.AppendFormat(" {0} {1}", SQLTokens.TOP, _topCount);
            }

            if (_fields.Count == 0) throw new Exception("SELECT statement is empty!");

            foreach (var field in _fields)
            {
                if (Fields.IndexOf(field) > 0)
                    sb.Append(",");

                sb.AppendFormat("\n\t{0}", field.Compile());
            }

            sb.AppendLine();

            if (!string.IsNullOrEmpty(_intoTable))
                sb.AppendFormat("{0} {1}\n", SQLTokens.INTO, _intoTable);

            if (_from.Tables.Count > 0)
                sb.AppendLine(_from.Compile());

            if (_where.LogicalClause.Operations.Count > 0)
                sb.AppendLine(_where.Compile());

            if (_groupby.Clauses.Count > 0)
                sb.AppendLine(_groupby.Compile());

            if (_orderby.Clauses.Count > 0)
                sb.AppendLine(_orderby.Compile());

            _compiled = sb.ToString();

            return sb.ToString();
        }

        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }


        public List<IDBDataSource> Tables
        {
            get { return _from.Tables; }
        }

        public DBField GetField(string name)
        {
            foreach (var table in Tables)
            {
                foreach (var field in table.Fields)
                {
                    if (table.Alias + '.' + field.Name == name)
                        return field;
                }
            }
            throw new Exception("Field not found");
        }
    }
}