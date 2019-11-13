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
           .ct_query()
             .m_from()
                .ld_table("T1")
                .@as("A")
                .ld_table("T2")
                .@as("B")
                .ld_param("param1")
                .ld_column("A", "F1")
                .eq()
                .join()
                .ct_query()
                    .m_select()
                        .ld_param("param2")
                        .ld_param("param3")
                        .add()
                        .@as("Summ")
                .st_query()
                .ld_column("Summ")
                .ld_column("A", "F1")
                .eq()
                .left_join()
            .m_where()
                .ld_column("A", "F1")
                .ld_column("B", "F1")
                .eq()
                .ld_column("A", "F2")
                .ld_column("B", "F2")
                .eq()
                .and()
            .m_group_by()
                .ld_column("A", "F1")
            .m_having()
                .ld_column("A", "F1")
                .sum()
                .ld_param("param4")
                .gt()
            .m_select()
                .ld_column("A", "F1")
                .@as("MyColumn")
            .st_query()
            ; 


            var a = 10;
        }
    }
}
