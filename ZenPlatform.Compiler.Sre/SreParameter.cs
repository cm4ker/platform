using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    public class SreParameter : IParameter
    {
        private readonly SreTypeSystem _ts;
        private readonly ParameterInfo _pi;

        public SreParameter(SreTypeSystem ts, ParameterInfo pi)
        {
            _ts = ts;
            _pi = pi;
        }


        public bool Equals(IParameter other)
        {
            throw new System.NotImplementedException();
        }

        public string Name => _pi.Name;
        public IType Type => _ts.ResolveType(_pi.ParameterType);
    }
}