using System;

namespace QueryCompiler
{
    public interface IToken : ICloneable
    {
        string Compile(bool recompile = false);
        string CompileExpression { get; set; }
    }
}