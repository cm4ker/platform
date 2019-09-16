using System;
using System.Reflection;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    public class SreParameter : IParameter
    {
        private readonly SreTypeSystem _ts;
        private readonly SreMethodBase _method;
        private readonly ParameterInfo _parameterInfo;


        internal SreParameter(SreTypeSystem ts, SreMethodBase method, ParameterInfo pi)
        {
            _ts = ts;
            _method = method;
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
        public int ArgIndex => _parameterInfo.Position + (_method.IsStatic ? 0 : 1);
    }

    internal class SreDefferedParameter : IParameter
    {
        private readonly SreMethodBuilder _parent;
        private readonly string _name;
        private readonly IType _type;
        private readonly int _sequence;
        private ParameterInfo _parameterInfo;

        public SreDefferedParameter(SreMethodBuilder parent, string name, IType type, int sequence)
        {
            _parent = parent;
            _name = name;
            _type = type;
            _sequence = sequence;
        }

        public bool Equals(IParameter other)
        {
            return this.Sequence == other.Sequence;
        }

        public ParameterInfo ParameterInfo =>
            _parameterInfo ??= TryGetParameterInfo() ??
                               throw new Exception("You must invoke BuildType() before construct");

        private ParameterInfo TryGetParameterInfo()
        {
            try
            {
                return ((SreParameter) _parent.Parameters[_sequence]).ParameterInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string Name => (_parent.IsBaked) ? _parent.Parameters[_sequence].Name : _name;
        public IType Type => (_parent.IsBaked) ? _parent.Parameters[_sequence].Type : _type;
        public int Sequence => _sequence;
        public int ArgIndex => _sequence + (_parent.Method.IsStatic ? 0 : 1);


        internal void Bake()
        {
            ((MethodBuilder) _parent.Method).DefineParameter(_sequence + 1, ParameterAttributes.None, _name);
        }
    }
}