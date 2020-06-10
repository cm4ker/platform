using System;
using System.Collections.Generic;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    /// <summary>
    /// This is for configuration methods description this is not a CLR method
    /// </summary>
    public abstract class PInvokable : PMember
    {
        internal PInvokable(Guid parentId, TypeManager tm) : this(Guid.NewGuid(), parentId, tm)
        {
        }

        internal PInvokable(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
        }


        public IEnumerable<PParameter> Parameters => TypeManager.FindParameters(this.Id);
    }
}