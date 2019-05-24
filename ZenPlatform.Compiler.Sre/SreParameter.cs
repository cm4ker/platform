using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    public class SreParameter : IParameter
    {
        private readonly SreTypeSystem _ts;
        private readonly ParameterInfo _parameterInfo;


        public SreParameter(SreTypeSystem ts, ParameterInfo pi)
        {
            _ts = ts;
            _parameterInfo = pi;
        }


        public bool Equals(IParameter other)
        {
            throw new System.NotImplementedException();
        }


        public ParameterInfo ParameterInfo => _parameterInfo;
        public string Name => ParameterInfo.Name;
        public IType Type => _ts.ResolveType(ParameterInfo.ParameterType);
        public int Sequence => _parameterInfo.Position;
    }
}