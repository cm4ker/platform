using System;
using System.IO;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.RoslynBackend
{
    public class RoslynConstructor : RoslynInvokableBase
    {
        public RoslynConstructor(RoslynTypeSystem ts, IMethodDefOrRef method, MethodDef methodDef, ITypeDefOrRef declType)
            : base(ts, method, methodDef,
                declType)
        {
        }

        public bool Equals(RoslynConstructor other)
        {
            throw new NotImplementedException();
        }

        public override void DumpRef(TextWriter tw)
        {
            DeclaringType.DumpRef(tw);
        }
    }
}