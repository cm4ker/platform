using System;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public abstract class PConstructor : PInvokable, IPConstructor
    {
        internal PConstructor(Guid parentId, TypeManager tm) : base(parentId, tm)
        {
        }

        internal PConstructor(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
        }

        public IConstructor BackendConstructor { get; set; }
    }
}