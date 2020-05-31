using System;

namespace Aquila.Compiler.Aqua.TypeSystem.Builders
{
    public class PConstructorBuilder : PConstructor
    {
        internal PConstructorBuilder(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
            Body = new CilBody();
        }

        public CilBody Body { get; }
    }
}