using System;
using ZenPlatform.QueryBuilder.Interfaces;

namespace ZenPlatform.QueryBuilder.Queries
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