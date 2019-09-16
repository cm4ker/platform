using System;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibParameter : IParameter
    {
        private Parameter _parameter;

        public DnlibParameter(DnlibTypeSystem typeSystem, MethodDef methodDef, Parameter parameter)
        {
            _parameter = parameter;
        }

        public bool Equals(IParameter other)
        {
            throw new NotImplementedException();
        }

        public string Name => _parameter.Name;
        public IType Type => null;
        public int Sequence => _parameter.Index;
        public int ArgIndex => _parameter.ParamDef.Sequence;
    }
}