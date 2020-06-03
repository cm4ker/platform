using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.Builders
{
    public class PConstructorBuilder : PConstructor
    {
        internal PConstructorBuilder(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
            Body = new PCilBody();
        }

        public PCilBody Body { get; }
    }
}