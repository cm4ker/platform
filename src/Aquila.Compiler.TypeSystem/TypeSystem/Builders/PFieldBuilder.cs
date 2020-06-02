using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.Builders
{
    public class PFieldBuilder : PField, IPFieldBuilder
    {
        internal PFieldBuilder(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
        }
    }
}