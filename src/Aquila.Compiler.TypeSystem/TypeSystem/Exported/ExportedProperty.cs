using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Aqua.TypeSystem.Exported
{
    public class ExportedProperty : PProperty
    {
        public ExportedProperty(IProperty backendProperty, TypeManager ts) : base(Guid.NewGuid(), Guid.Empty, ts)
        {
        }
    }
}