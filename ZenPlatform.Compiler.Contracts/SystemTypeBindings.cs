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

        private const string MSCORLIB =
            "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        private const string SYSTEM_NAMESPACE = "System";

        public SystemTypeBindings(ITypeSystem ts)
        {
            _ts = ts;
        }

        public IType Int => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Int32)}", MSCORLIB);

        public IType String => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(String)}", MSCORLIB);

        public IType Char => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Char)}", MSCORLIB);

        public IType Bool => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Bool)}", MSCORLIB);

        public IType Double => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Double)}", MSCORLIB);

        public IType Guid => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Guid)}", MSCORLIB);

        public IType Void => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Void)}", MSCORLIB);

        public IType Object => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Object)}", MSCORLIB);
    }
}