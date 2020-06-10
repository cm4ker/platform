using System;
using System.IO;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynConstructor : RoslynInvokableBase, IConstructor
    {
        public RoslynConstructor(RoslynTypeSystem ts, IMethodDefOrRef method, MethodDef methodDef,
            ITypeDefOrRef declType)
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

        public bool Equals(IConstructor other)
        {
            throw new NotImplementedException();
        }
    }
}