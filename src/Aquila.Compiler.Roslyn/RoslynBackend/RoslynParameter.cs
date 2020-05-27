using System;
using System.IO;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;
using dnlib.DotNet;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynParameter : IParameter
    {
        private readonly MethodDef _methodDef;
        private Parameter _parameter;

        private RoslynContextResolver _cr;

        public RoslynParameter(RoslynTypeSystem typeSystem, MethodDef methodDef, ModuleDef module, Parameter parameter)
        {
            _methodDef = methodDef;
            _parameter = parameter;
            _cr = new RoslynContextResolver(typeSystem, module);
        }

        public Parameter Parameter => _parameter;

        public bool Equals(RoslynParameter other)
        {
            throw new NotImplementedException();
        }

        public string Name => (string.IsNullOrEmpty(_parameter.Name)) ? _parameter.ToString() : _parameter.Name;
        public IType Type => _cr.GetType(_parameter.Type);
        public int Sequence => _parameter.Index;
        public int ArgIndex => _parameter.Index;

        public void Dump(TextWriter tw)
        {
            Type.DumpRef(tw);
            tw.Space().W(Name);
        }

        public void DumpRef(TextWriter tw)
        {
            tw.W(Name);
        }

        public bool Equals(IParameter other)
        {
            throw new NotImplementedException();
        }
    }
}