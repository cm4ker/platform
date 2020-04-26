using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    [DebuggerDisplay("{_method}")]
    class SreMethodBase : SreMemberInfo
    {
        private readonly MethodBase _method;

        private IReadOnlyList<IParameter> _parameters;

        public SreMethodBase(SreTypeSystem system, MethodBase method) : base(system, method)
        {
            _method = method;
        }

        public bool IsPublic => _method.IsPublic;
        public bool IsStatic => _method.IsStatic;

        public IReadOnlyList<IParameter> Parameters => _parameters ??= _method.GetParameters()
            .Select(p => new SreParameter(System, this, p)).ToList();
    }
}