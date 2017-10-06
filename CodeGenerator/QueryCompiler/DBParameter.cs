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

        public string Name => _parameter.ParameterName;

        public void SetValue(object value)
        {
            _parameter.Value = value;
        }

        public DbParameter SqlParameter => _parameter;

        public override string Compile(bool recompile = false)
        {
            return _parameter.ParameterName;
        }
    }
}