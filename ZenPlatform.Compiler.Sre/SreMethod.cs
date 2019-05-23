using System.Diagnostics;
using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    [DebuggerDisplay("{Method}")]
    class SreMethod : SreMethodBase, IMethod
    {
        public MethodInfo Method { get; }
        private readonly SreTypeSystem _system;

        public SreMethod(SreTypeSystem system, MethodInfo method) : base(system, method)
        {
            Method = method;
            _system = system;
        }

        public bool Equals(IMethod other) => ((SreMethod) other)?.Method.Equals(Method) == true;
        public IType ReturnType => _system.ResolveType(Method.ReturnType);
        public IType DeclaringType => _system.ResolveType(Method.DeclaringType);
    }
}