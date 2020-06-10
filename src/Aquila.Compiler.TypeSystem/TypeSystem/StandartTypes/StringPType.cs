using System;
using Aquila.Compiler.Aqua.TypeSystem.Exported;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.StandartTypes
{
    /// <summary>
    ///  Тип строки
    /// </summary>
    public sealed class StringPType : PExportedType
    {
        internal StringPType(TypeManager ts, IType type) : base(ts, type)
        {
            ts.AddOrUpdateSetting(new ObjectSetting {ObjectId = Id, SystemId = TypeConstants.String.sysId});
        }

        public override Guid Id => TypeConstants.String.guid;

        public override bool IsAbstract => true;
        public override bool IsPrimitive => true;

        public override PrimitiveKind PrimitiveKind => PrimitiveKind.String;

        public override string Name
        {
            get { return "String"; }
        }
    }
}