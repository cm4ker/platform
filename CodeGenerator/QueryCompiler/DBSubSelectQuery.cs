using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryCompiler
{
    public class DBSubSelectQuery : IDBTableDataSorce
    {
        private readonly DBSelectQuery _query;
        private string _compileExpression;
        private string _alias;
        private readonly List<DBClause> _fields;

        public DBSubSelectQuery(DBSelectQuery query, string alias)
        {
            _query = query;
            _fields = new List<DBClause>();

            SetAliase(alias);
            foreach (var field in _query.Fields)
            {
                if (field as DBSelectField is null) continue;

                var dbField = field as DBSelectField;

                DBSelectField newField = DBClause.CreateSelectField(this, dbField.Schema, dbField.Name) as DBSelectField;

                if (!string.IsNullOrEmpty(dbField.Alias))
                    newField = DBClause.CreateSelectField(this, dbField.Schema, dbField.Alias) as DBSelectField;

                _fields.Add(newField);
            }

            _compileExpression = "({Query})";
        }



        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            return StandartCompilers.CompileAliasedObject(_compileExpression, new { Query = _query.Compile(), Alias = _alias });
        }

        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }

        public void SetAliase(string alias)
        {
            _alias = alias;
            foreach (DBSelectField field in _fields)
            {
                field.SetAliase($"{field.Owner.Alias}.{field.Name}");
            }
        }

        public string Alias
        {
            get { return _alias; }
        }

        public List<DBClause> Fields

        {
            get { return _fields; }
        }

        public DBClause GetField(string name)
        {
            return _fields.Cast<DBSelectField>().First(x => x.Name == name);
        }
    }
}