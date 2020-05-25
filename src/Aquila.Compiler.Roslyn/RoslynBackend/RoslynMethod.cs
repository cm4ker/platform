using System.IO;
using dnlib.DotNet;
using IMethod = Aquila.Compiler.Contracts.IMethod;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynMethod : RoslynInvokableBase, IMethod
    {
        public RoslynMethod(RoslynTypeSystem ts, dnlib.DotNet.IMethod method, MethodDef methodDef,
            ITypeDefOrRef declaringType) : base(ts, method, methodDef,
            declaringType)
        {
        }


        public override void DumpRef(TextWriter tw)
        {
            base.DumpRef(tw);

            if (IsGeneric)
                using (tw.AngleBrace())
                {
                    var wasFirst = false;
                    foreach (var arg in GenericArguments)
                    {
                        if (wasFirst)
                            tw.W(",");

                        arg.DumpRef(tw);

                        wasFirst = true;
                    }
                }
        }
    }
}