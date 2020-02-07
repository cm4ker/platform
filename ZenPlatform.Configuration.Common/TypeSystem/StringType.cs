using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.TypeSystem
{
    /// <summary>
    ///  Тип строки
    /// </summary>
    public class StringType : Type
    {
        internal StringType(ITypeManager ts) : base(ts)
        {
        }

        public override uint SystemId => 6;

        public override Guid Id => new Guid(1, 2, 4, 56, 72, 234, 234, 23, 123, 12, 6);
        public override bool IsAbstract => true;
        public override bool IsPrimitive => true;
        public override PrimitiveKind PrimitiveKind => PrimitiveKind.String;

        public override string Name
        {
            get { return "String"; }
        }
    }
}