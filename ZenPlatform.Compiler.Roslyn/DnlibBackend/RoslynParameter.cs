using System;
using System.IO;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class RoslynParameter
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
        public RoslynType Type => _cr.GetType(_parameter.Type);
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
    }
}