using System;
using System.Reflection.Emit;
using Aquila.Compiler.Aqua;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Dnlib;
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
            ts.SetFullName("F1.F2.f2.MyClass");
            ts.SetScope(ScopeAffects.Code);

            var complexType = tm.DefineTypeSet();
            complexType.AddType(tm.Int);
            complexType.AddType(tm.String);

            var m = ts.DefineMethod();
            m.SetName("SomeMethod");
            m.SetReturnType(complexType);
            var body = m.Body;


            var cyclestart = body.DefineLabel();
            var condition = body.DefineLabel();

            var iloc = body.DefineLocal(tm.Int);
            var aloc = body.DefineLocal(tm.Int);

            body
                
                .LdcI4(0)
                .StLoc(aloc)
                
                .LdcI4(0)
                .StLoc(iloc)
                
                .Br(condition)

                //loop start
                .MarkLabel(cyclestart)
                .LdcI4(2)
                .LdLoc(aloc)
                .Add()
                .StLoc(aloc)
                //inc i
                .LdLoc(iloc)
                .LdcI4(1)
                .Add()
                
                .StLoc(iloc)
                
                .MarkLabel(condition)
                //compare
                .LdLoc(iloc)
                .LdcI4(10)
                .Clt()
                
                .BrTrue(cyclestart)
                
                //loop end
                .LdLoc(aloc)
                .Ret();

            BackendCompilation cb = new BackendCompilation();
            cb.MakeMiracle(tm, asm);

            asm.Write("compile.bll");
        }
    }
}