using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Cli.Builder
{
    public class XCCompiller
    {
        private readonly XCRoot _root;
        private readonly string _outputDirectory;

        public XCCompiller(XCRoot root, string outputDirectory)
        {
            _root = root;
            _outputDirectory = outputDirectory;
        }

        public void Build()
        {
            IAssemblyPlatform pl = new CecilAssemblyPlatform();
            var asm = pl.CreateAssembly("Build");

            //STAGE0
            foreach (var t in _root.Data.ComponentTypes)
            {
                t.Parent.ComponentImpl.Generator.Stage0(t, asm);
            }

            var list = new Dictionary<XCObjectTypeBase, IType>();

            //STAGE1
            foreach (var t in _root.Data.ComponentTypes)
            {
                var b = t.Parent.ComponentImpl.Generator.Stage1(t, asm);
                list.Add(t, b);
            }

            //STAGE2
            foreach (var t in _root.Data.ComponentTypes)
            {
                t.Parent.ComponentImpl.Generator.Stage2(t, (ITypeBuilder) list[t], list.ToImmutableDictionary(), asm);
            }

            asm.Write(_outputDirectory + "\\Build.dll");
        }
    }
}