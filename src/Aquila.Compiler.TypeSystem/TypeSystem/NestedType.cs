using System;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    /// <summary>
    /// For define tables and some complex objects
    /// </summary>
    public class NestedType : PType
    {
        public NestedType(Guid parentId, TypeManager ts) : base(ts)
        {
            ParentId = parentId;
        }

        public override Guid? ParentId { get; }

        public override bool IsNestedType => true;
    }
}