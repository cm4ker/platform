using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    public class CecilParameter : IParameter
    {
        private readonly CecilTypeSystem _ts;
        private readonly ParameterDefinition _pi;

        public CecilParameter(CecilTypeSystem ts, ParameterDefinition pi)
        {
            _ts = ts;
            _pi = pi;
        }


        public bool Equals(IParameter other)
        {
            throw new System.NotImplementedException();
        }

        public string Name => _pi.Name;
        public IType Type => _ts.Resolve(_pi.ParameterType);
    }
}