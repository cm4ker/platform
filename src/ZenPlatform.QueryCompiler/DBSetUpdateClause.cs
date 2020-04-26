using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Interfaces;

namespace ZenPlatform.QueryBuilder
{
    public class DBSetUpdateClause : IDBToken
    {
        private Dictionary<DBTableField, DBClause> _fields;
        private DBParameterCollection _parameters;


        public DBSetUpdateClause()
        {
            _fields = new Dictionary<DBTableField, DBClause>();
            _parameters = new DBParameterCollection();
        }

        public void AddField(DBTableField field)
        {
             var paramName = $"{field.Name}_{DBHelper.GetRandomString(DBHelper.RandomCharsInParams())}";
            var parameter = new DBParameter(paramName, field.Schema.Type, field.Schema.IsNullable);
            _fields.Add(field, parameter);
            _parameters.Add(parameter);
        }

        public void AddField(DBTableField destField, DBClause value)
        {
            _fields.Add(destField, value);
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
            var index = 0;
            foreach (var item in _fields)
            {
                if (index > 0) sb.Append(",");
                sb.AppendLine($"{item.Key.Name} = {item.Value.Compile(recompile)}");
                index++;
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