using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ZenPlatform.QueryBuilder.Tests
{
    public class QueryMachineTest
    {
        [Fact]
        public void SelectTest()
        {
            var machine = new QueryMachine();

            machine
                .ld_table("sdasd")
                .ld_column("asdasd")
                
                ;


        }
    }
}
