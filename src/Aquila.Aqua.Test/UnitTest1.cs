using System;
using Aquila.Compiler.Aqua;
using Aquila.Compiler.Aqua.TypeSystem;
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
            TypeManager tm = new TypeManager(new RoslynTypeSystem(new RoslynPlatformFactory(), null));

            var ts = tm.DefineType();
            ts.SetName("Class");
            ts.SetScope(ScopeAffects.Code);

            var m = ts.DefineMethod();

            m.SetName("Some method");
            m.SetReturnType(tm.Int);


            var myType = tm.DefineType();
        }
    }
}