using System;
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

            var ts = tm.TypeSet();
            ts.AddType(tm.Int);
            ts.AddType(tm.String);
            ts.AddType(tm.Numeric);
            ts.AddType(tm.Boolean);


            var myType = tm.Type();
        }
    }
}