using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.Exported
{
    /// <summary>
    /// Exported from another libraries (mscorlib, etc...)
    /// This type is Already sealed (immutable)
    /// </summary>
    public class PExportType : PType
    {
        private readonly IType _backendType;
        private Guid _id;

        private IList<ExportedProperty> _properties;

        public PExportType(TypeManager ts, IType backendType) : base(ts)
        {
            _backendType = backendType;
            _id = Guid.NewGuid();
        }

        public override Guid Id => _id;

        public IType BackendType => _backendType;

        public override IEnumerable<IPProperty> Properties
            => _properties ?? _backendType.Properties.Select(x => new ExportedProperty(x, (TypeManager) TypeManager))
                .ToList();
    }
}