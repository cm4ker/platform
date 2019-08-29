using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Compiler.Platform
{
    public class XCCompiller
    {

        public XCCompiller()
        {

        }

        public string Build(XCRoot root, string outputDirectory, string buildName)
        {
            IAssemblyPlatform pl = new CecilAssemblyPlatform();
            var asm = pl.CreateAssembly(buildName);

            //STAGE0
            foreach (var t in root.Data.ComponentTypes)
            {
                t.Parent.ComponentImpl.Generator.Stage0(t, asm);
            }

            var list = new Dictionary<XCObjectTypeBase, IType>();

            //STAGE1
            foreach (var t in root.Data.ComponentTypes)
            {
                var b = t.Parent.ComponentImpl.Generator.Stage1(t, asm);
                list.Add(t, b);
            }

            //STAGE2
            foreach (var t in root.Data.ComponentTypes)
            {
                t.Parent.ComponentImpl.Generator.Stage2(t, (ITypeBuilder) list[t], list.ToImmutableDictionary(), asm);
            }
            var buildFIlePath = Path.Combine(outputDirectory, $"{buildName}.dll");
            asm.Write(buildFIlePath);

            return buildFIlePath;


        }
    }
}