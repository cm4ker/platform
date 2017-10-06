using System;

namespace QueryCompiler
{
    public abstract class DBQuery : IToken
    {
        public string Compile(bool recompile = false)
        {
            throw new NotImplementedException();
        }

        public string CompileExpression { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}