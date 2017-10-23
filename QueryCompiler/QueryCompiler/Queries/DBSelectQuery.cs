using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using QueryCompiler.Interfaces;

namespace QueryCompiler.Queries
{
    public class DBSelectQuery : IDBTablesContainer, IDBFieldContainer, IQueryable, IParametrized
    {
        private string _compileExpression;

        private string _intoTable;
        private uint _topCount;

        private DbCommand _dbCommand;

        private List<DBClause> _fields;

        private DBFromClause _from;
        private DBWhereClause _where;
        private DBOrderByClause _orderby;
        private DBGroupByClause _groupby;


        private string _compiled;

        internal DBSelectQuery()
        {
            _fields = new List<DBClause>();
            _from = new DBFromClause();
            _where = new DBWhereClause();
            _orderby = new DBOrderByClause();
            _groupby = new DBGroupByClause();
            _dbCommand = new SqlCommand();
        }

        public void Select(DBClause clause)
        {
            ProcessNewClause(clause);
            _fields.Add(clause);
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

        public DBSelectQuery From(DBTable table)
        {
            _from.AddTable(table);
            return this;
        }

        public DBSelectQuery From(DBSubSelectQuery table)
        {
            _from.AddTable(table);
            return this;
        }

        public DBSelectQuery Into(string intoTableName)
        {
            _intoTable = intoTableName;
            return this;
        }

        public DBOrderByClause OrderBy(DBClause clause)
        {
            ProcessNewClause(clause);
            _orderby.ThenOrderBy(clause);
            return _orderby;
        }

        public DBOrderByClause OrderByDesc(DBClause clause)
        {
            ProcessNewClause(clause);
            _orderby.ThenOrderByDesc(clause);
            return _orderby;
        }

        public DBGroupByClause GroupBy(DBClause clause)
        {
            ProcessNewClause(clause);
            _groupby.ThenGroupBy(clause);
            return _groupby;
        }

        private void ProcessNewClause(DBClause clause)
        {
            var param = clause as DBParameter;
            if (param != null)
            {
                _dbCommand.Parameters.Add(param.SqlParameter);
            }
        }

        public DBLogicalOperation Where(DBClause clause1, CompareType type, DBClause clause2)
        {
            ProcessNewClause(clause1);
            ProcessNewClause(clause2);
            return _where.Where(clause1, type, clause2);
        }

        public DBLogicalOperation WhereNot(DBClause clause1, CompareType type, DBClause clause2)
        {
            ProcessNewClause(clause1);
            ProcessNewClause(clause2);
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

        public List<DBClause> Fields
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

        public DbCommand GetDbCommand()
        {
            _dbCommand.CommandText = Compile();
            return _dbCommand;
        }

        public List<IDBTableDataSource> Tables
        {
            get { return _from.Tables; }
        }

        public DBClause GetField(string name)
        {
            foreach (var table in Tables)
            {
                foreach (var field in table.Fields)
                {
                    if (table.Alias + '.' + (field as DBField).Name == name)
                        return field;
                }
            }
            throw new Exception("Field not found");
        }
    }
}