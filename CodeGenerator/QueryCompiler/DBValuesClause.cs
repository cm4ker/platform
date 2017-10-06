using System;
using System.Collections.Generic;
using System.Text;

namespace QueryCompiler
{
    public class DBValuesClause : IToken
    {
        private List<DBTableField> _fields;
        private List<DBParameter> _parameters;


        public DBValuesClause()
        {
            _fields = new List<DBTableField>();
            _parameters = new List<DBParameter>();
        }

        public void AddField(DBTableField field)
        {
            _fields.Add(field);
            var paramName = $"{field.Name}_{DBHelper.GetRandomString(12)}";
            _parameters.Add(new DBParameter(paramName, field.Schema.Type));
        }

        public List<DBTableField> Fields
        {
            get { return _fields; }
        }

        public List<DBParameter> Parameters => _parameters;

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(SQLTokens.VALUES);
            sb.Append("(");
            foreach (var field in _fields)
            {
                var param = _parameters[_fields.IndexOf(field)];

                sb.Append($"{param.Name},");

            }
            return sb.ToString().Trim(',') + ")";
        }

        public string CompileExpression { get; set; }
    }
}