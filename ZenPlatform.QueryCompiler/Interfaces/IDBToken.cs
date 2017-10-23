using System;

namespace ZenPlatform.QueryCompiler.Interfaces
{
    public interface IDBToken : ICloneable
    {
        string Compile(bool recompile = false);
        string CompileExpression { get; set; }
    }
}