using System;

namespace Aquila.Compiler.Aqua.TypeSystem.Builders
{
    public class PFieldBuilder : PField
    {
        internal PFieldBuilder(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
        }
    }
}