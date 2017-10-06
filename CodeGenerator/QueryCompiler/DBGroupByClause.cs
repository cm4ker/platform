using System;

namespace QueryCompiler
{
    public class DBGroupByClause : IToken
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            throw new Exception();
        }

        public string CompileExpression { get; set; }
    }
}