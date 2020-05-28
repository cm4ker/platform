using System;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    /// <summary>
    /// For define tables and some complex objects
    ///
    /// Tables has its own properties, methods, can be convertable etc. It is common 
    /// </summary>
    public class NestedType : PType
    {
        public NestedType(Guid id, Guid parentId, TypeManager ts) : base(ts)
        {
            Id = id;
            ParentId = parentId;
        }

        public override Guid Id { get; }

        public override Guid? ParentId { get; }

        public override bool IsNestedType => true;
    }
}