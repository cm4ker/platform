using System.Data;
using System.Text;
using ZenPlatform.QueryBuilder.Interfaces;

namespace ZenPlatform.QueryBuilder.Schema
{
    public class DBFieldSchema: IDBToken
    {
        private DBType _type;
        private bool _isKey;
        private bool _isUnique;
        private string _columnName;
        private int _numericPrecision;
        private int _numericScale;
        private bool _isIdentity;
        private bool _isNullable;
        private int _columnSize;

        public DBFieldSchema(DBType type, string columnName, int columnSize, int numericPrecision, int numericScale, bool isIdentity, bool isKey = false, bool isUnique = false, bool isNullable = false)
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

        public DBType Type
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

        public bool IsIdentity
        {
            get { return _isIdentity; }
            set { _isIdentity = value; }
        }

        public string CompileExpression { get; set; }

        public object Clone()
        {
            throw new System.NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            //sb.Append("{0} {1} {2}[{3}]", SQLTokens.DROP, SQLTokens.DATABASE, DropIfExist ? $"{SQLTokens.IF} {SQLTokens.EXISTS} " : "", _databaseName);
            return sb.ToString();
        }
    }
}