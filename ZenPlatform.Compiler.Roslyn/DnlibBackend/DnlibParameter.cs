using System;
using System.IO;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreParameter
    {
        private readonly MethodDef _methodDef;
        private Parameter _parameter;

        private SreContextResolver _cr;

        public SreParameter(SreTypeSystem typeSystem, MethodDef methodDef, ModuleDef module, Parameter parameter)
        {
            _methodDef = methodDef;
            _parameter = parameter;
            _cr = new SreContextResolver(typeSystem, module);
        }

        public Parameter Parameter => _parameter;

        public bool Equals(SreParameter other)
        {
            throw new NotImplementedException();
        }

        public string Name => (string.IsNullOrEmpty(_parameter.Name)) ? _parameter.ToString() : _parameter.Name;
        public SreType Type => _cr.GetType(_parameter.Type);
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