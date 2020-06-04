using System;
using System.Reflection.Emit;
using Aquila.Compiler.Aqua;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Xunit;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Aqua.Test
{
    public class UnitTest1
    {
        [Fact]
        public void TypeSystemCreate()
        {
            IAssemblyPlatform ap = new RoslynAssemblyPlatform();
            var bts = ap.CreateTypeSystem();

            TypeManager tm = new TypeManager(bts);
            var asm = ap.AsmFactory.CreateAssembly(bts, "Test", new Version("1.0.0.0"));

            var ts = tm.DefineType();
            ts.SetName("MyClass");
            ts.SetScope(ScopeAffects.Code);

            var complexType = tm.DefineTypeSet();
            complexType.AddType(tm.Int);
            complexType.AddType(tm.String);

            var m = ts.DefineMethod();
            m.SetName("SomeMethod");
            m.SetReturnType(complexType);
            var body = m.Body;

            body.LdArg_0()
                .Call(m)
                .Ret();

            BackendCompilation cb = new BackendCompilation();
            cb.MakeMiracle(tm, asm);

            asm.Write("compile.bll");
        }
    }
}