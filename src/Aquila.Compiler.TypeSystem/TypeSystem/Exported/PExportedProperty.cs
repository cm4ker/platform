using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Aqua.TypeSystem.Exported
{
    public class PExportedProperty : PProperty
    {
        public PExportedProperty(IProperty backendProperty, TypeManager ts) : base(Guid.NewGuid(), Guid.Empty, ts)
        {
        }
    }
}