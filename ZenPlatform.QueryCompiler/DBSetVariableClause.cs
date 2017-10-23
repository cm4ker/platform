using System;
using System.Text;
using ZenPlatform.QueryCompiler.Queries;

namespace ZenPlatform.QueryCompiler
{
    public class DBSetVariableClause : IQueryable
    {
        private readonly DBVariable _variable;
        private readonly DBClause _value;

        public DBSetVariableClause(DBVariable variable, DBClause value)
        {
            _variable = variable;
            _value = value;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(SQLTokens.SET);
            sb.AppendFormat(" {0} {1}", _variable.Compile(), _value.Compile());

            return sb.ToString();
        }

        public string CompileExpression { get; set; }
    }
}