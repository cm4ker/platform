using System.Data;

namespace QueryCompiler.Schema
{
    public class DBFieldSchema
    {
        private SqlDbType _type;
        private bool _isKey;
        private bool _isUnique;
        private bool _isForeignKey;
        private string _columnName;
        private bool _isNullable;

        public DBFieldSchema(SqlDbType type, string columnName, bool isKey = false, bool isUnique = false, bool isNullable = false)
        {
            _type = type;
            _isKey = isKey;
            _isUnique = isUnique;
            _columnName = columnName;
            _isNullable = isNullable;
        }

        public string ColumnName => _columnName;

        public bool IsKey
        {
            get { return _isKey; }
            set { _isKey = value; }
        }

        public bool IsUnique
        {
            get { return _isUnique; }
            set { _isUnique = value; }
        }

        public bool IsNullable
        {
            get { return _isNullable; }
            set { _isNullable = value; }
        }

        public SqlDbType Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}