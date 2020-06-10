using System;
using Aquila.Compiler.Aqua.TypeSystem.Exported;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    /// <summary>
    ///  Тип строки
    /// </summary>
    public sealed class ObjectPType : PExportedType
    {
        internal ObjectPType(TypeManager ts, IType type) : base(ts, type)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = TypeConstants.Object.sysId});
        }

        public override Guid Id => TypeConstants.Object.guid;

        public override bool IsAbstract => true;
        public override bool IsPrimitive => true;

        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Unknown;

        public override string Name
        {
            get { return "Object"; }
        }
    }

    /// <summary>
    ///  Тип строки
    /// </summary>
    public sealed class VoidPType : PExportedType
    {
        internal VoidPType(TypeManager ts, IType type) : base(ts, type)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = TypeConstants.Void.sysId});
        }

        public override Guid Id => TypeConstants.Void.guid;

        public override bool IsAbstract => true;
        public override bool IsPrimitive => true;

        public override PrimitiveKind PrimitiveKind => PrimitiveKind.Unknown;

        public override string Name => "Void";
    }
}