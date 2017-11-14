using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.QueryBuilder.Queries
{
    public class DBAlterTableQuery : IQueryable
    {

        private List<DBField> _addColumns;
        private List<DBField> _dropColumns;
        private List<DBField> _alterColumns;
        private DBTable _table;

        public string CompileExpression { get; set; }

        public DBAlterTableQuery()
        {
            _addColumns = new List<DBField>();
            _dropColumns = new List<DBField>();
            _alterColumns = new List<DBField>();

        }

        public DBAlterTableQuery(DBTable table) : this()
        {
            _table = table;
        }

        public DBAlterTableQuery(string tableName) : this()
        {
            _table = new DBTable(tableName);
        }

        public DBAlterTableQuery AlterColumn(DBField field)
        {
            _alterColumns.Add(field);
            return this;
        }

        public DBAlterTableQuery AlterColumns(IEnumerable<DBField> fields)
        {
            _alterColumns.AddRange(fields);
            return this;
        }

        public DBAlterTableQuery AlterColumn(DBType type, string columnName, int columnSize, int numericPrecision, int numericScale, bool isIdentity, bool isKey = false, bool isUnique = false, bool isNullable = false)
        {
            _alterColumns.Add(new DBTableField(_table, columnName)
            {
                Schema = new DBFieldSchema(type,  columnSize, numericPrecision, numericScale, isIdentity, isKey, isUnique, isNullable)
            });
            return this;
        }

        public DBAlterTableQuery DropColumn(string fieldName)
        {
            _dropColumns.Add(new DBTableField(_table, fieldName));
            return this;
        }

        public DBAlterTableQuery DropColumn(DBField field)
        {
            _dropColumns.Add(field);
            return this;
        }

        public DBAlterTableQuery DropColumns(IEnumerable<DBField> fields)
        {
            _dropColumns.AddRange(fields);
            return this;
        }

        public DBAlterTableQuery AddColumn(DBField field)
        {
            _addColumns.Add(field);
            return this;
        }

        public DBAlterTableQuery AddColumns(IEnumerable<DBField> fields)
        {
            _addColumns.AddRange(fields);
            return this;
        }

        public DBAlterTableQuery AddColumn(DBType type, string columnName, int columnSize, int numericPrecision, int numericScale, bool isIdentity, bool isKey = false, bool isUnique = false, bool isNullable = false)
        {
            _addColumns.Add(new DBTableField(_table, columnName)
            {
                Schema = new DBFieldSchema(type, columnSize, numericPrecision, numericScale, isIdentity, isKey, isUnique, isNullable)
            });
            return this;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} [{2}] \n", SQLTokens.ALTER, SQLTokens.TABLE, _table.Name);
            if (_addColumns.Count > 0)
            { 
                foreach (var field in _addColumns)
                {

                    sb.AppendFormat("{0} {1} {2}{3}", SQLTokens.ADD, field.Name, field.Schema.Compile(),
                        _addColumns.IndexOf(field) != _addColumns.Count - 1 ? "," : ";");

                }
                sb.AppendLine("\n");

                return sb.ToString();
            }

            if (_dropColumns.Count > 0)
            {
                foreach (var field in _dropColumns)
                {

                    sb.AppendFormat("{0} {1} {2}{3}", SQLTokens.DROP, SQLTokens.COLUMN, field.Name,
                        _dropColumns.IndexOf(field) != _dropColumns.Count - 1 ? ",\n" : ";");

                }
                sb.AppendLine("\n");

                return sb.ToString();
            }

            if (_alterColumns.Count > 0)
            {
                foreach (var field in _alterColumns)
                {

                    sb.AppendFormat("{0} {1} {2} {3}{4}", SQLTokens.ALTER, SQLTokens.COLUMN, field.Name, field.Schema.Type.Compile(),
                        _alterColumns.IndexOf(field) != _alterColumns.Count - 1 ? "," : ";");

                }
                sb.AppendLine("\n");

                return sb.ToString();
            }

            throw new Exception("The instruction must contain drop, alter or add columns.");
        }
    }
}
