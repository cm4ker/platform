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

        private List<PExportedProperty> _properties;
        private List<PExportedMethod> _methods;
        private List<PExportedConstructor> _constructors;

        public PExportType(TypeManager ts, IType backendType) : base(ts)
        {
            _backendType = backendType;
            _id = Guid.NewGuid();
        }

        public override Guid Id => _id;

        internal override IType BackendType => _backendType;

        public override IEnumerable<PProperty> Properties
            => _properties ?? _backendType.Properties.Select(x => new PExportedProperty(x, (TypeManager) TypeManager))
                .ToList();

        public override IEnumerable<PMethod> Methods
            => _methods ?? _backendType.Methods.Select(x => new PExportedMethod(x, (TypeManager) TypeManager)).ToList();

        public override IEnumerable<PConstructor> Constructors
            => _constructors ?? _backendType.Constructors
                .Select(x => new PExportedConstructor(x, (TypeManager) TypeManager))
                .ToList();
    }
}