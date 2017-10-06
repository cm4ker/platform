using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace QueryCompiler
{
    public class DBSetUpdateClause : IToken
    {
        private List<DBTableField> _fields;
        private List<DBParameter> _parameters;


        public DBSetUpdateClause()
        {
            _fields = new List<DBTableField>();
            _parameters = new List<DBParameter>();
        }

        public void AddField(DBTableField field)
        {
            _fields.Add(field);
            var paramName = $"{field.Name}_{DBHelper.GetRandomString(DBHelper.RandomCharsInParams())}";
            _parameters.Add(new DBParameter(paramName, field.Schema.Type));
        }

        public List<DBParameter> Parameters
        {
            get { return _parameters; }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(SQLTokens.SET);
            foreach (var field in _fields)
            {
                var param = _parameters[_fields.IndexOf(field)];
                if (_fields.IndexOf(field) > 0) sb.Append(",");
                sb.AppendLine($"{field.Name} = {param.Name}");

                //new SqlParameter($"@{paramName}", field.Schema.Type)
            }
            return sb.ToString();
        }

        public string CompileExpression { get; set; }
    }

    public class DBVariable : IToken
    {
        private readonly string _variableName;

        public DBVariable(string variableName)
        {
            _variableName = variableName;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            return _variableName;
        }

        public string CompileExpression { get; set; }
    }
}