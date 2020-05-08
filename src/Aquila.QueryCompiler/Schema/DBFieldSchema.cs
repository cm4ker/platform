using System.Data;
using System.Text;
using Aquila.QueryBuilder.Interfaces;

namespace Aquila.QueryBuilder.Schema
{
    public class DBFieldSchema: IDBToken
    {
        private DBType _type;
        private bool _isKey;
        private bool _isUnique;
        private int _numericPrecision;
        private int _numericScale;
        private bool _isIdentity;
        private bool _isNullable;
        private int _columnSize;

        public DBFieldSchema(DBType type, int columnSize, int numericPrecision, int numericScale, bool isIdentity, bool isKey = false, bool isUnique = false, bool isNullable = false)
        {
            _columnSize = columnSize;
            _type = type;
            _isKey = isKey;
            _isUnique = isUnique;

            _numericPrecision = numericPrecision;
            _numericScale = numericScale;
            IsIdentity = isIdentity;
            _isNullable = isNullable;
        }


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

        private string CompileType()
        {
            var sb = new StringBuilder();

            sb.Append(_type.Compile());
            if (_columnSize > 0)
            {
                sb.Append($"({_columnSize})");
            }

            if (_numericPrecision > 0 && _numericScale > 0)
            {
                sb.Append($"({_numericPrecision},{_numericScale})");
            }

            return sb.ToString();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}{1}{2}{3}", CompileType(), _isIdentity? $" {SQLTokens.IDENTITY}(1,1)": "", 
                _isNullable ? "" : $" {SQLTokens.NOT}", $" {SQLTokens.NULL}", _isKey ? $" {SQLTokens.PRIMARY} {SQLTokens.KEY}":"");
            return sb.ToString();
        }

        
        public bool Equals(DBFieldSchema schema)
        {

            if (schema == null || GetType() != schema.GetType())
            {
                return false;
            }

            return schema.IsIdentity == this.IsIdentity && schema.IsKey == this.IsKey && schema.IsNullable == this.IsNullable && schema.IsUnique == this.IsUnique
                && schema.NumericPrecision == this.NumericPrecision && schema.NumericScale == this.NumericScale && schema.ColumnSize == this.ColumnSize
                && schema.Type == this.Type;
        }
    }
}