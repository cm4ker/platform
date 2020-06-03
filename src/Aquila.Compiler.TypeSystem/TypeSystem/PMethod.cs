using System;
using System.Collections.Generic;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PMethod : PInvokable, IPMethod
    {
        internal PMethod(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
        }

        internal PMethod(Guid parentId, TypeManager tm) : this(Guid.NewGuid(), parentId, tm)
        {
        }

        public virtual IPType ReturnType => TypeManager.Unknown;

        public IMethod BackendMethod { get; set; }
    }
}