using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Interfaces;

namespace Aquila.QueryBuilder
{
    public class DBValuesClause : IDBToken
    {
        private List<DBField> _fields;
        private List<DBParameter> _parameters;


        public DBValuesClause()
        {
            _fields = new List<DBField>();
            _parameters = new List<DBParameter>();
        }

        public void AddField(DBField field)
        {
            _fields.Add(field);
            var paramName = $"{field.Name}";
            _parameters.Add(new DBParameter(paramName, field.Schema.Type, field.Schema.IsNullable));
        }

        public List<DBField> Fields
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
            sb.Append("\t(");
            foreach (var field in _fields)
            {
                var param = _parameters[_fields.IndexOf(field)];

                sb.Append($"{param.Compile()},");

            }
            return sb.ToString().Trim(',') + ")";
        }

        public string CompileExpression { get; set; }
    }
}