using System;
using System.Collections.Generic;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    /// <summary>
    /// Биндинги на системные типы платформы
    /// </summary>
    public sealed class SystemTypeBindings
    {
        private readonly RoslynTypeSystem _ts;

        public string MSCORLIB { get; } = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        public string PLATFORM_CORE { get; } =
            "ZenPlatform.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        public string PLATFORM_DATA_COMPONENT { get; }
            = "ZenPlatform.DataComponent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        private const string SYSTEM_NAMESPACE = "System";

        internal SystemTypeBindings(RoslynTypeSystem ts)
        {
            _ts = ts;

            Methods = new SystemMethods(ts, this);
        }


        public RoslynTypeSystem TypeSystem => _ts;

        public RoslynType Int => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Int32)}", MSCORLIB);

        public RoslynType IntPrt => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(IntPtr)}", MSCORLIB);

        public RoslynType Int64 => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Int64)}", MSCORLIB);

        public RoslynType String => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(String)}", MSCORLIB);

        public RoslynType Char => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Char)}", MSCORLIB);

        public RoslynType Boolean => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Boolean)}", MSCORLIB);

        public RoslynType Double => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Double)}", MSCORLIB);

        public RoslynType Guid => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Guid)}", MSCORLIB);

        public RoslynType Void => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Void)}", MSCORLIB);

        public RoslynType Object => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(Object)}", MSCORLIB);

        public RoslynType Type => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Type)}", MSCORLIB);

        public RoslynType Byte => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Byte)}", MSCORLIB);

        public RoslynType DateTime => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.DateTime)}", MSCORLIB);

        public RoslynType List => _ts.FindType(typeof(List<>).FullName, MSCORLIB);

        public RoslynType IEnumerable => _ts.FindType(typeof(IEnumerable<>).FullName, MSCORLIB);

        public RoslynType Exception => _ts.FindType($"{SYSTEM_NAMESPACE}.{nameof(System.Exception)}", MSCORLIB);


        public RoslynType DbCommand => _ts.Resolve<System.Data.Common.DbCommand>();

        // public RoslynType Client => _ts.Resolve<IProtocolClient>();
        //
        // public RoslynType ServerInitializer => _ts.Resolve<IServerInitializer>();
        //
        // public RoslynType InvokeService => _ts.Resolve<IInvokeService>();
        //
        // public RoslynType InvokeContext => _ts.Resolve<InvokeContext>();
        //
        // public RoslynType Route => _ts.Resolve<Route>();
        //
        // public RoslynType ParametricMethod => _ts.Resolve<ParametricMethod>();

        public RoslynType Session => _ts.FindType($"ZenPlatform.Core.Sessions.Session", PLATFORM_CORE);

        public RoslynType Reference =>
            _ts.FindType($"ZenPlatform.DataComponent.Interfaces.IReference", PLATFORM_DATA_COMPONENT);

        // public SreType MultSreType => _ts.FindType<UnionType>();
        //
        // public SreType UnionTypeStorage => _ts.FindType<UnionTypeStorage>();
        //


        public RoslynType Action => _ts.Resolve<Action>();


        public SystemMethods Methods { get; }


        public class SystemMethods
        {
            private readonly RoslynTypeSystem _ts;
            private readonly SystemTypeBindings _stb;

            internal SystemMethods(RoslynTypeSystem ts, SystemTypeBindings stb)
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