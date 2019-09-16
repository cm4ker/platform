using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    public class CecilParameter : IParameter
    {
        private readonly CecilTypeSystem _ts;
        private readonly MethodDefinition _md;
        private readonly ParameterDefinition _pd;
        private CecilContextResolver _cr;

        public CecilParameter(CecilTypeSystem ts, MethodDefinition md, ParameterDefinition pi)
        {
            _ts = ts;
            _md = md;
            _pd = pi.Resolve();

            _cr = new CecilContextResolver(ts, md.Module);

            _cr.GetReference((ITypeReference) _cr.GetType(_pd.ParameterType));
        }

        public ParameterDefinition ParameterDefinition => _pd;


        public string Name => ParameterDefinition.Name;

        public IType Type => _cr.GetType(ParameterDefinition.ParameterType);

        public int Sequence => _pd.Sequence;
        public int ArgIndex => _pd.Sequence;

        public bool Equals(IParameter other)
        {
            throw new System.NotImplementedException();
        }
    }
}