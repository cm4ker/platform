using System;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibParameter : IParameter
    {
        public DnlibParameter(DnlibTypeSystem typeSystem, MethodDef methodDef, Parameter parameter)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IParameter other)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
        public IType Type { get; }
        public int Sequence { get; }
        public int ArgIndex { get; }
    }
}