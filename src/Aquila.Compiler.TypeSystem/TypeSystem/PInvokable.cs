using System;
using System.Collections.Generic;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    /// <summary>
    /// This is for configuration methods description this is not a CLR method
    /// </summary>
    public class PInvokable : PMember, IPInvokable
    {
        internal PInvokable(Guid parentId, TypeManager tm) : base(parentId, tm)
        {
        }

        internal PInvokable(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
        }

        public IEnumerable<IPArgument> Arguments => TypeManager.FindArguments(this.Id);
    }
}