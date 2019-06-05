using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    public interface ISreMethod
    {
        MethodInfo Method { get; }
    }

    [DebuggerDisplay("{Method}")]
    class SreMethod : SreMethodBase, IMethod, ISreMethod
    {
        public MethodInfo Method { get; }
        readonly SreTypeSystem _system;

        protected SreTypeSystem TypeSystem => _system;


        public SreMethod(SreTypeSystem system, MethodInfo method) : base(system, method)
        {
            Method = method;
            _system = system;
        }

        public bool Equals(IMethod other) => ((SreMethod) other)?.Method.Equals(Method) == true;
        public IType ReturnType => _system.ResolveType(Method.ReturnType);
        public IType DeclaringType => _system.ResolveType(Method.DeclaringType);

        public IMethod MakeGenericMethod(IType[] typeArguments)
        {
            var sreTypes = typeArguments.Select(x => _system.GetType(x)).ToArray();
            return new SreMethod(_system, Method.MakeGenericMethod(sreTypes));
        }
    }
}