using System;
using System.Reflection;

namespace ZenPlatform.Compiler.Contracts
{
    /// <summary>
    /// Биндинги на системные типы платформы
    /// </summary>
    public class SystemTypeBindings
    {
        private readonly ITypeSystem _ts;
        private const string SYSTEM_NAMESPACE = "System";

        public SystemTypeBindings(ITypeSystem ts)
        {
            _ts = ts;
        }

        public IType Int => _ts.FindType(SYSTEM_NAMESPACE, nameof(Int));

        public IType String => _ts.FindType(SYSTEM_NAMESPACE, nameof(String));

        public IType Char => _ts.FindType(SYSTEM_NAMESPACE, nameof(Char));

        public IType Bool => _ts.FindType(SYSTEM_NAMESPACE, nameof(Bool));

        public IType Double => _ts.FindType(SYSTEM_NAMESPACE, nameof(Double));

        public IType Guid => _ts.FindType(SYSTEM_NAMESPACE, nameof(Guid));

        public IType Void => _ts.FindType(SYSTEM_NAMESPACE, nameof(Void));

        public IType Object => _ts.FindType(SYSTEM_NAMESPACE, nameof(Object));
    }
}