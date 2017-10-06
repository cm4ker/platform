using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace QueryCompiler
{
    public class DBParameter : DBClause
    {
        private readonly DbParameter _parameter;

        public DBParameter(string name, SqlDbType type)
        {
            _parameter = new SqlParameter('@' + name.Trim('@'), type);
        }

        public DBParameter(string name, object value)
        {
            _parameter = new SqlParameter('@' + name.Trim('@'), value);
        }

        public DBParameter(string name, SqlDbType type, bool isNullable)
        {
            _parameter = new SqlParameter('@' + name.Trim('@'), type);
            _parameter.IsNullable = isNullable;
            //_parameter.SourceColumnNullMapping = isNullable;

        }

        public DBParameter(string name, object value, SqlDbType type, int size, bool isNullable, short precision, short scale, string sourceColumnName)
        {
            _parameter = new SqlParameter('@' + name.Trim('@'), type, size, ParameterDirection.Input, isNullable, (byte)precision, (byte)scale, sourceColumnName, DataRowVersion.Default, value);
        }

        public string Name => _parameter.ParameterName;

        public void SetValue(object value)
        {
            if (value is null)
                _parameter.Value = DBNull.Value;
            else
                _parameter.Value = value;
        }

        public DbParameter SqlParameter => _parameter;

        public override string Compile(bool recompile = false)
        {
            return _parameter.ParameterName;
        }

        public override bool Equals(object obj)
        {
            var parameter = obj as DBParameter;
            if (parameter is null) return false;

            return parameter.Name.ToLower() == this.Name.ToLower();
        }
    }
}