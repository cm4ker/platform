using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Aquila.QueryBuilder.Schema;

namespace Aquila.QueryBuilder.Queries
{
    public class DBCreateTableQuery : IQueryable
    {
        private List<DBField> _fields;
        private DBTable _table;

        public DBCreateTableQuery()
        {
            _fields = new List<DBField>();
        }

        public DBCreateTableQuery(DBTable table) : this()
        {
            _table = table;
            foreach (var field in table.Fields)
            {
                _fields.Add(field);
            }
        }

        public DBCreateTableQuery(string tableName):this()
        {
            _table = new DBTable(tableName);
        }

        public DBCreateTableQuery Table(string tableName)
        {
            _table = new DBTable(tableName);
            return this;
        }

        public DBCreateTableQuery Field(string columnName, DBType type, int size, short numericPrecision, short numericScale, bool isIdentity, bool isUnique, bool isKey, bool isNullable)
        {
            var field = new DBTableField(_table, columnName);
            field.Schema = new DBFieldSchema(type, size, numericPrecision, numericScale, isIdentity, isKey, isUnique);
            _fields.Add(field);
            return this;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            if (!_fields.Any()) throw new Exception("You MUST declare any fields");

            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} [{2}] (\n", SQLTokens.CREATE, SQLTokens.TABLE, _table.Name);

            foreach (var field in _fields)
            {
                sb.AppendFormat("\t[{0}] {1}{2}\n", field.Name, field.Schema.Compile(), _fields.IndexOf(field) != _fields.Count -1 ? "," : "");
            }
            sb.AppendLine(")");

            return sb.ToString();
        }

        public string CompileExpression { get; set; }
    }
}
