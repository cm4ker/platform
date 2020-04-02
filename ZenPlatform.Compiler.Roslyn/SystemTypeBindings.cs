using System;
using System.Collections.Generic;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Contracts.Network;
using ZenPlatform.Core.Network;
using SreType = ZenPlatform.Compiler.Roslyn.DnlibBackend.SreType;

namespace ZenPlatform.Compiler.Roslyn
{
    /// <summary>
    /// Биндинги на системные типы платформы
    /// </summary>
    public sealed class SystemTypeBindings
    {
        private readonly SreTypeSystem _ts;

        public string MSCORLIB { get; } = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        public string PLATFORM_CORE { get; } =
            "ZenPlatform.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        public string PLATFORM_DATA_COMPONENT { get; }
            = "ZenPlatform.DataComponent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        private const string SYSTEM_NAMESPACE = "System";

        internal SystemTypeBindings(SreTypeSystem ts)
        {
            _ts = ts;

            Methods = new SystemMethods(ts, this);
        }


        public SreTypeSystem TypeSystem => _ts;

        public SreType Int => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Int32)}", MSCORLIB);

        public SreType IntPrt => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(IntPtr)}", MSCORLIB);

        public SreType Int64 => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Int64)}", MSCORLIB);

        public SreType String => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(String)}", MSCORLIB);

        public SreType Char => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Char)}", MSCORLIB);

        public SreType Boolean => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Boolean)}", MSCORLIB);

        public SreType Double => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Double)}", MSCORLIB);

        public SreType Guid => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Guid)}", MSCORLIB);

        public SreType Void => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Void)}", MSCORLIB);

        public SreType Object => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Object)}", MSCORLIB);

        public SreType Type => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Type)}", MSCORLIB);

        public SreType Byte => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Byte)}", MSCORLIB);

        public SreType DateTime => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.DateTime)}", MSCORLIB);

        public SreType List => _ts.FindType(typeof(List<>).FullName, MSCORLIB);

        public SreType IEnumerable => _ts.FindType(typeof(IEnumerable<>).FullName, MSCORLIB);

        public SreType Exception => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Exception)}", MSCORLIB);


        public SreType DbCommand => _ts.Resolve<System.Data.Common.DbCommand>();

        public SreType Client => _ts.Resolve<IProtocolClient>(); // _ts.FindType<Client>()

        public SreType ServerInitializer => _ts.Resolve<IServerInitializer>();

        public SreType InvokeService => _ts.Resolve<IInvokeService>();

        public SreType InvokeContext => _ts.Resolve<InvokeContext>();

        public SreType Route => _ts.Resolve<Route>();

        public SreType Session => _ts.FindType($"ZenPlatform.Core.Sessions.Session", PLATFORM_CORE);

        public SreType Reference =>
            _ts.FindType($"ZenPlatform.DataComponent.Interfaces.IReference", PLATFORM_DATA_COMPONENT);

        // public SreType MultSreType => _ts.FindType<UnionType>();
        //
        // public SreType UnionTypeStorage => _ts.FindType<UnionTypeStorage>();
        //
        public SreType ParametricMethod => _ts.Resolve<ParametricMethod>();

        public SreType Action => _ts.Resolve<Action>();


        public SystemMethods Methods { get; }


        public class SystemMethods
        {
            private readonly SreTypeSystem _ts;
            private readonly SystemTypeBindings _stb;

            internal SystemMethods(SreTypeSystem ts, SystemTypeBindings stb)
            {
                _ts = ts;
                _stb = stb;
            }

            // public SreMethod Concat => _stb.String.FindMethod(x =>
            //     x.Name == nameof(string.Concat) && x.Parameters.Count == 2
            //                                     && x.Parameters[0].Type == _stb.String &&
            //                                     x.Parameters[1].Type == _stb.String);
        }
    }
}