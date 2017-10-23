using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryCompiler.Interfaces;

namespace ZenPlatform.QueryCompiler
{
    public class DBSetUpdateClause : IDBToken
    {
        private List<DBTableField> _fields;
        private DBParameterCollection _parameters;


        public DBSetUpdateClause()
        {
            _fields = new List<DBTableField>();
            _parameters = new DBParameterCollection();
        }

        public void AddField(DBTableField field)
        {
            _fields.Add(field);
            var paramName = $"{field.Name}_{DBHelper.GetRandomString(DBHelper.RandomCharsInParams())}";
            _parameters.Add(new DBParameter(paramName, field.Schema.Type, field.Schema.IsNullable));
        }

        public DBParameterCollection Parameters
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

    public class DBVariable : IDBToken
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