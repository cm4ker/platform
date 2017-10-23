using System;
using ZenPlatform.QueryCompiler.Interfaces;

namespace ZenPlatform.QueryCompiler.Queries
{
    public abstract class DBQuery : IDBToken
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