using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    /// <summary>
    /// Exported from another libraries (mscorlib, etc...)
    /// This type is Already sealed (immutable)
    /// </summary>
    public class PExportType : PType
    {
        private readonly IType _backendType;
        private Guid _id;

        public PExportType(TypeManager ts, IType backendType) : base(ts)
        {
            _backendType = backendType;
            _id = Guid.NewGuid();
        }

        public override Guid Id => _id;

        public IType BackendType => _backendType;
    }
}