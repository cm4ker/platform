using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZenPlatform.QueryCompiler.Interfaces;
using ZenPlatform.QueryCompiler.Queries;

namespace ZenPlatform.QueryCompiler
{
    public class DBSelectPipe : IDBFieldContainer, IParametrized
    {
        private readonly Dictionary<DBSelectQuery, DBSelectConjunctionTypes> _queryes;
        private readonly List<DBClause> _fields;

        public DBSelectPipe()
        {
            _queryes = new Dictionary<DBSelectQuery, DBSelectConjunctionTypes>();
            _fields = new List<DBClause>();
        }

        public DBSubSelectQuery AsSubQuery(string alias)
        {
            DBSubSelectQuery result = new DBSubSelectQuery(this, alias);
            return result;
        }

        private void UpdateFields()
        {

            var fields = _queryes.FirstOrDefault().Key?.Fields;
            if (fields != null)
                foreach (var field in fields)
                {
                    if (field is DBSelectField)
                    {

                        var dbField = field as DBSelectField;

                        DBSelectField newField = DBClause.CreateSelectField(null, dbField.Schema, dbField.Name) as DBSelectField;

                        if (!string.IsNullOrEmpty(dbField.Alias))
                            newField = DBClause.CreateSelectField(null, dbField.Schema, dbField.Alias) as DBSelectField;

                        _fields.Add(newField);
                    }
                    else if (field is DBField)
                    {
                        var dbField = field as DBField;

                        DBSelectField newField = dbField.ToSelectField();

                        _fields.Add(newField);
                    }
                }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            foreach (var query in _queryes)
            {
                switch (query.Value)
                {
                    case DBSelectConjunctionTypes.Union: sb.AppendLine((new DBUnionClause()).Compile()); break;
                    case DBSelectConjunctionTypes.UnionAll: sb.AppendLine((new DBUnionAllClause()).Compile()); break;
                    case DBSelectConjunctionTypes.None: break;
                    default: throw new NotSupportedException($"This {query.Value} value is not supported");
                }
                sb.AppendLine(query.Key.Compile());
            }

            return sb.ToString();
        }

        public string CompileExpression { get; set; }

        public void Union(DBSelectQuery query)
        {
            if (!_queryes.Any())
            {
                _queryes.Add(query, DBSelectConjunctionTypes.None);
                UpdateFields();
            }
            else
                _queryes.Add(query, DBSelectConjunctionTypes.Union);

        }

        public void UnionAll(DBSelectQuery query)
        {
            if (!_queryes.Any())
            {
                _queryes.Add(query, DBSelectConjunctionTypes.None);
                UpdateFields();
            }
            else
                _queryes.Add(query, DBSelectConjunctionTypes.UnionAll);

        }

        public List<DBClause> Fields
        {
            get { return _fields; }
        }

        public DBClause GetField(string name)
        {
            if (!_queryes.Any()) throw new FieldNotFoundException(name);
            return _queryes.First().Key.GetField(name);
        }

        public List<IDBTableDataSource> Tables
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public IDBTableDataSource GetTable(string alias)
        {
            throw new NotImplementedException();
        }

        public DBParameterCollection Parameters
        {
            get
            {
                var parameters = new DBParameterCollection();
                foreach (var query in _queryes)
                {
                    parameters.AddRange(query.Key.Parameters, true);
                }
                return parameters;

            }
        }
    }
}