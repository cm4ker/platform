using System;
using Aquila.Compiler.Aqua.TypeSystem;
using Xunit;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Aqua.Test
{
    public class UnitTest1
    {
        [Fact]
        public void TypeSystemCreate()
        {
            TypeManager tm = new TypeManager();

            var ts = tm.TypeSet();
            ts.AddType(tm.Int);
            ts.AddType(tm.String);
            ts.AddType(tm.Numeric);
            ts.AddType(tm.Boolean);


            var myType = tm.Type();

            myType. = "MyType";
        }
    }
}