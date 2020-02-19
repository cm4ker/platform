using System;
using System.Collections.Generic;
using System.Reflection;
using ZenPlatform.Compiler.Infrastructure;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Contracts.Network;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Network.Contracts;

namespace ZenPlatform.Compiler.Contracts
{
    /// <summary>
    /// Биндинги на системные типы платформы
    /// </summary>
    public sealed class SystemTypeBindings
    {
        private readonly ITypeSystem _ts;

        public string MSCORLIB { get; } = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        public string PLATFORM_CORE { get; } =
            "ZenPlatform.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        public string PLATFORM_DATA_COMPONENT { get; }
            = "ZenPlatform.DataComponent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        private const string SYSTEM_NAMESPACE = "System";

        internal SystemTypeBindings(ITypeSystem ts)
        {
            _ts = ts;

            Methods = new SystemMethods(ts, this);
        }


        public ITypeSystem TypeSystem => _ts;

        public IType Int => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Int32)}", MSCORLIB);

        public IType Int64 => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Int64)}", MSCORLIB);

        public IType String => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(String)}", MSCORLIB);

        public IType Char => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Char)}", MSCORLIB);

        public IType Boolean => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Boolean)}", MSCORLIB);

        public IType Double => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Double)}", MSCORLIB);

        public IType Guid => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Guid)}", MSCORLIB);

        public IType Void => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Void)}", MSCORLIB);

        public IType Object => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Object)}", MSCORLIB);

        public IType Type => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Type)}", MSCORLIB);

        public IType Byte => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Byte)}", MSCORLIB);

        public IType DateTime => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.DateTime)}", MSCORLIB);

        public IType List => _ts.FindType(typeof(List<>).FullName, MSCORLIB);

        public IType IEnumerable => _ts.FindType(typeof(IEnumerable<>).FullName, MSCORLIB);

        public IType Exception => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Exception)}", MSCORLIB);


        public IType DbCommand => _ts.FindType<System.Data.Common.DbCommand>();

        public IType Client => _ts.FindType<IProtocolClient>(); // _ts.FindType<Client>()

        public IType ServerInitializer => _ts.FindType<IServerInitializer>();

        public IType InvokeService => _ts.FindType<IInvokeService>();

        public IType InvokeContext => _ts.FindType<InvokeContext>();

        public IType Route => _ts.FindType<Route>();

        public IType Session => _ts.FindType($"ZenPlatform.Core.Sessions.Session", PLATFORM_CORE);

        public IType Reference =>
            _ts.FindType($"ZenPlatform.DataComponent.Interfaces.IReference", PLATFORM_DATA_COMPONENT);

        public IType MultiType => _ts.FindType<UnionType>();

        public IType UnionTypeStorage => _ts.FindType<UnionTypeStorage>();

        public IType ParametricMethod => _ts.FindType<ParametricMethod>();


        public SystemMethods Methods { get; }


        public class SystemMethods
        {
            private readonly ITypeSystem _ts;
            private readonly SystemTypeBindings _stb;

            internal SystemMethods(ITypeSystem ts, SystemTypeBindings stb)
            {
                _ts = ts;
                _stb = stb;
            }

            public IMethod Concat => _stb.String.FindMethod(x =>
                x.Name == nameof(string.Concat) && x.Parameters.Count == 2
                                                && x.Parameters[0].Type == _stb.String &&
                                                x.Parameters[1].Type == _stb.String);
        }
    }
}