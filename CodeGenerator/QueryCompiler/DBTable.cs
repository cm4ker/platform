using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using QueryCompiler.Schema;

namespace QueryCompiler
{
    public class DBTable : IDBTableDataSorce
    {
        private readonly string _name;
        private readonly List<DBClause> _fields;
        private string _compileExpression;
        private string _alias;
        private readonly List<DBFieldSchema> _fieldsSchema;

        public DBTable(string name, DBSchemaManager manager, string alias = "")
        {
            _name = name;
            _fields = new List<DBClause>();
            _compileExpression = "[{Name}]";
            _alias = alias;
            _fieldsSchema = manager.GetTableSchema(this);
        }

        public DBTableField DeclareField(string name)
        {
            var schema = _fieldsSchema.Find(x => x.ColumnName.ToLower() == name.ToLower());
            var result = new DBTableField(this, name, schema);
            _fields.Add(result);
            return result;
        }

        public void FillFieldsFromSchema()
        {
            foreach (var schema in _fieldsSchema)
            {
                var result = new DBTableField(this, schema.ColumnName, schema);
                _fields.Add(result);
            }
        }

        public List<DBFieldSchema> SchemaFields => _fieldsSchema;

        public string Compile(bool recompile = false)
        {
            return StandartCompilers.CompileAliasedObject(_compileExpression, new { Name = _name, Alias = _alias });
        }

        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }

        public void SetAliase(string alias)
        {
            _alias = alias;
        }

        public string Alias
        {
            get { return _alias; }
        }

        public string Name
        {
            get { return _name; }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public List<DBClause> Fields
        {
            get { return _fields; }
        }

        public void Validate()
        {

        }

        public DBClause GetField(string name)
        {
            return _fields.First(x => (x as DBTableField).Name.ToLower() == name.ToLower());
        }
    }
}