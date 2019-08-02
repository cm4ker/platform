using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Cli.Builder
{
    public class ConfBuilder
    {
        public ConfBuilder(XCRoot conf)
        {
            IAssemblyPlatform pl = new CecilAssemblyPlatform();
            var asm = pl.CreateAssembly("Debug");

            //STAGE0
            foreach (var t in conf.Data.ComponentTypes)
            {
                t.Parent.ComponentImpl.Generator.Stage0(t, asm);
            }

            var list = new Dictionary<XCObjectTypeBase, IType>();

            //STAGE1
            foreach (var t in conf.Data.ComponentTypes)
            {
                var b = t.Parent.ComponentImpl.Generator.Stage1(t, asm);
                list.Add(t, b);
            }

            //STAGE2
            foreach (var t in conf.Data.ComponentTypes)
            {
                t.Parent.ComponentImpl.Generator.Stage2(t, (ITypeBuilder) list[t], list.ToImmutableDictionary(), asm);
            }

            asm.Write("Debug.bll");
        }
    }
}