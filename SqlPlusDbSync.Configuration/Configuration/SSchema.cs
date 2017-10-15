using System;
using System.Data;

namespace SqlPlusDbSync.Platform.Configuration
{
    public class SSchema
    {
        private SqlDbType _type;
        private bool _isKey;
        private bool _isUnique;
        private string _columnName;
        private int _numericPrecision;
        private int _numericScale;
        private bool _isNullable;
        private int _columnSize;
        private Type _clrType;

        public SSchema(SqlDbType type, string columnName, int columnSize, int numericPrecision, int numericScale, bool isKey = false, bool isUnique = false, bool isNullable = false)
        {
            _columnSize = columnSize;
            _type = type;
            _isKey = isKey;
            _isUnique = isUnique;
            _columnName = columnName;
            _numericPrecision = numericPrecision;
            _numericScale = numericScale;
            _isNullable = isNullable;
            _clrType = PlatformHelper.GetClrType(type, _isNullable);
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

        public int NumericPrecision
        {
            get { return _numericPrecision; }
            set { _numericPrecision = value; }
        }

        public int NumericScale
        {
            get { return _numericScale; }
            set { _numericScale = value; }
        }

        public Type ClrType
        {
            get { return _clrType; }
            set { _clrType = value; }
        }
    }
}