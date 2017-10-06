using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace QueryCompiler
{
    public class DBSelectQuery : IDBTableDataSourceContainer
    {
        private string _compileExpression;

        private string _selectExpression;
        private string _fromExpression;
        private string _whereExpression;
        private string _groupByExpression;
        private string _havingExpression;
        private string _orderbyExpression;

        private int _topCount;

        private List<DBClause> _fields;
        private IDBTableDataSorce _rootTable;
        private readonly List<IDBTableDataSorce> _tables;

        private DBFromClause _from;
        private DBWhereClause _where;

        private DBOrderByClause _orderby;

        private string _compiled;

        public DBSelectQuery()
        {
            _fields = new List<DBClause>();
            _compileExpression = $"{{SelectExpression}}\n{{FromExpression}}\n{{WhereExpression}}\n{{GroupByExpression}}\n{{HavingExpression}}\n{{OrderByExpression}}";
            _from = new DBFromClause();
            _where = new DBWhereClause();
            _orderby = new DBOrderByClause();
        }

        public void Select(DBField field)
        {
            _fields.Add(field);
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

        public DBSelectQuery Top(int rowsCount)
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

        public DBOrderByClause OrderBy(DBClause clause)
        {
            _orderby.ThenOrderBy(clause);
            return _orderby;
        }


        public DBLogicalOperation Where(DBClause clause1, CompareType type, DBClause clause2)
        {
            return _where.Where(clause1, type, clause2);
        }


        public DBParameterCollection Parameters => _where.Parameters;

        private IDBTableDataSorce FindTableDataSourceByAliase(string alias)
        {
            return _from.Tables.Find(x => x.Alias == alias);
        }

        public DBJoinClause Join(IDBTableDataSorce table, JoinType joinType = JoinType.Left)
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
            return this.MemberwiseClone();
        }

        public string Compile(bool recompile = false)
        {
            if (!string.IsNullOrEmpty(_compiled))
                return _compiled;

            if (_fields is null)
                throw new Exception();

            _selectExpression = $"{SQLTokens.SELECT} ";
            if (_topCount != 0)
                _selectExpression += $"{SQLTokens.TOP} {_topCount}";

            foreach (var field in _fields)
            {
                _selectExpression += "\n\t" + field.Compile() + ',';
            }
            _selectExpression = _selectExpression.Trim(',');

            if (_from.Tables.Count > 0)
                _fromExpression = _from.Compile();

            if (_where.LogicalClause.Operations.Count > 0)
                _whereExpression = _where.Compile();

            if (_orderby.Clauses.Count > 0)
                _orderbyExpression = _orderby.Compile();

            _compiled = StandartCompilers.SimpleCompiler(_compileExpression,
                new
                {
                    SelectExpression = _selectExpression,
                    FromExpression = _fromExpression,
                    WhereExpression = _whereExpression,
                    GroupByExpression = _groupByExpression,
                    HavingExpression = _havingExpression,
                    OrderByExpression = _orderbyExpression
                });
            return _compiled;
        }

        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }

        public List<IDBTableDataSorce> Tables
        {
            get { return _from.Tables; }
        }

        public IDBTableDataSorce RootTable
        {
            get { return _from.RootTalbe; }
        }

        public IDBTableDataSorce GetTable(string alias)
        {
            return Tables.Find(x => x.Alias == alias);
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