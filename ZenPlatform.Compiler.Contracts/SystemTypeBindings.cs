using System;
using System.Reflection;
using ZenPlatform.AsmClientInfrastructure;
using ZenPlatform.Compiler.Infrastructure;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Compiler.Contracts
{
    /// <summary>
    /// Биндинги на системные типы платформы
    /// </summary>
    public sealed class SystemTypeBindings
    {
        private readonly ITypeSystem _ts;

        private const string MSCORLIB =
            "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        private const string SYSTEM_NAMESPACE = "System";

        internal SystemTypeBindings(ITypeSystem ts)
        {
            _ts = ts;
        }


        private IType FindType<T>()
        {
            return FindType(typeof(T));
        }

        private IType FindType(Type type)
        {
            var name = type.Name;
            var @namespace = type.Namespace;
            var assembly = type.Assembly.GetName().FullName;

            return _ts.FindType($"{@namespace}.{name}", assembly);
        }

        public ITypeSystem TypeSystem => _ts;

        public IType Int => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Int32)}", MSCORLIB);

        public IType String => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(String)}", MSCORLIB);

        public IType Char => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Char)}", MSCORLIB);

        public IType Boolean => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Boolean)}", MSCORLIB);

        public IType Double => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Double)}", MSCORLIB);

        public IType Guid => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Guid)}", MSCORLIB);

        public IType Void => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Void)}", MSCORLIB);

        public IType Object => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Object)}", MSCORLIB);

        public IType Type => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Type)}", MSCORLIB);

        public IType Client => FindType<Client>();

        public IType MultiType => FindType<UnionType>();

        public IType UnionTypeStorage => FindType<UnionTypeStorage>();
    }
}