using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    public class CecilParameter : IParameter
    {
        private readonly CecilTypeSystem _ts;
        private readonly MethodDefinition _md;
        private readonly ParameterDefinition _pd;

        public CecilParameter(CecilTypeSystem ts, MethodDefinition md, ParameterDefinition pi)
        {
            _ts = ts;
            _md = md;
            _pd = pi;
        }

        public ParameterDefinition ParameterDefinition => _pd;

        public string Name => ParameterDefinition.Name;

        public IType Type => _ts.Resolve(ParameterDefinition.ParameterType);

        public int Sequence => _pd.Sequence;
        public int ArgIndex => _pd.Sequence + (_md.IsStatic ? 0 : 1);

        public bool Equals(IParameter other)
        {
            throw new System.NotImplementedException();
        }
    }
}