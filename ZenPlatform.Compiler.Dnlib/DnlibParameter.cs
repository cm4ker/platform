using System;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibParameter : IParameter
    {
        private Parameter _parameter;
        private DnlibContextResolver _cr;

        public DnlibParameter(DnlibTypeSystem typeSystem, MethodDef methodDef, Parameter parameter)
        {
            _parameter = parameter;
            _cr = new DnlibContextResolver(typeSystem, methodDef.Module);
        }

        public bool Equals(IParameter other)
        {
            throw new NotImplementedException();
        }

        public string Name => _parameter.Name;
        public IType Type => _cr.GetType(_parameter.Type);
        public int Sequence => _parameter.Index;
        public int ArgIndex => _parameter.ParamDef.Sequence;
    }
}