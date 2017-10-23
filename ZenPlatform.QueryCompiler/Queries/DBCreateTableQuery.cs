using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using ZenPlatform.QueryCompiler.Schema;

namespace ZenPlatform.QueryCompiler.Queries
{
    public class DBCreateTableQuery : IQueryable
    {
        private string _tableName;
        private List<DBFieldSchema> _fields;


        internal DBCreateTableQuery()
        {
            _fields = new List<DBFieldSchema>();
        }

        public DBCreateTableQuery Table(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        public DBCreateTableQuery Field(string fieldName, Type type, int size, short numericPrecision, short numericScale, bool isIdentity, bool isUnique, bool isKey, bool isNullable)
        {
            //TODO: Реализовать правильное добавление полей и компиляцию запроса
            _fields.Add(new DBFieldSchema(DBHelper.GetDBType(type), fieldName, size, numericPrecision, numericScale, isIdentity, isKey, isUnique));
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
            sb.AppendFormat("{0} {1} [{2}] (\n\t", SQLTokens.CREATE, SQLTokens.TABLE, _tableName);

            foreach (var field in _fields)
            {
                if (_fields.IndexOf(field) > 0)
                    sb.Append(",");
                sb.AppendFormat("[{0}] {1}\n\t", field.ColumnName, field.Type.Compile());
            }
            sb.AppendLine(")");

            return sb.ToString();
        }

        public string CompileExpression { get; set; }
    }
}
