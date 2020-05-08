using System;

namespace Aquila.QueryBuilder.Interfaces
{
    public interface IDBToken : ICloneable
    {
        string Compile(bool recompile = false);
        string CompileExpression { get; set; }
    }
}