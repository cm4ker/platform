using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using QueryCompiler.Schema;

namespace QueryCompiler.Queries
{
    public class DBCreateTableQuery : IQueryable
    {
        private string _tableName;
        private DBTable _table;
        private List<DBFieldSchema> _fields;


        internal DBCreateTableQuery()
        {
            _fields = new List<DBFieldSchema>();
        }

        public DBCreateTableQuery Table(string tableName)
        {
            _table = new DBTable(tableName, _fields);
            _tableName = tableName;
            return this;
        }

        public DBCreateTableQuery Field(string fieldName, Type type, int size, short numericPrecision, short numericScale, bool isIdentity, bool isUnique, bool isKey, bool isNullable)
        {
            _fields.Add(new DBFieldSchema(DBHelper.GetSqlType(type), fieldName, size, numericPrecision, numericScale, isIdentity, isKey, isUnique));
            return this;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            throw new NotImplementedException();
        }

        public string CompileExpression { get; set; }
    }
}
