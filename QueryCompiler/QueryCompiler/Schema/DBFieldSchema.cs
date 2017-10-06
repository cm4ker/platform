using System.Data;

namespace QueryCompiler.Schema
{
    public class DBFieldSchema
    {
        private SqlDbType _type;
        private bool _isKey;
        private bool _isUnique;
        private string _columnName;
        private short _numericPrecision;
        private short _numericScale;
        private bool _isIdentity;
        private bool _isNullable;
        private int _columnSize;

        public DBFieldSchema(SqlDbType type, string columnName, int columnSize, short numericPrecision, short numericScale, bool isIdentity, bool isKey = false, bool isUnique = false, bool isNullable = false)
        {
            _columnSize = columnSize;
            _type = type;
            _isKey = isKey;
            _isUnique = isUnique;
            _columnName = columnName;
            _numericPrecision = numericPrecision;
            _numericScale = numericScale;
            IsIdentity = isIdentity;
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

        public int ColumnSize
        {
            get { return _columnSize; }
            set { _columnSize = value; }
        }

        public SqlDbType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public short NumericPrecision
        {
            get { return _numericPrecision; }
            set { _numericPrecision = value; }
        }

        public short NumericScale
        {
            get { return _numericScale; }
            set { _numericScale = value; }
        }

        public bool IsIdentity
        {
            get { return _isIdentity; }
            set { _isIdentity = value; }
        }
    }
}