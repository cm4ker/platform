using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    /// <summary>
    ///  Тип строки
    /// </summary>
    public sealed class StringPType : PType
    {
        internal StringPType(TypeManager ts) : base(ts)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = 6});
        }

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