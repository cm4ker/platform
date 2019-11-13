using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;

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
                .ld_column("F1", "B")
                .ld_column("F1", "A")
                .eq()
                .join()
                .ct_query()
                    .m_select()
                        .ld_param("param2")
                        .ld_param("param3")
                        .add()
                        .@as("Summ")
                .st_query()
                .@as("subQuery")
                .ld_column("Summ", "subQuery")
                .ld_column("F2", "A")
                .eq()
                .left_join()
            .m_where()
                .ld_column("F1", "A")
                .ld_column("F1", "B")
                .eq()
                .ld_column("F2", "A")
                .ld_column("F2", "B")
                .eq()
                .and()
            .m_group_by()
                .ld_column("F1", "A")
            .m_having()
                .ld_column("F1", "A")
                .sum()
                .ld_param("param4")
                .gt()
            .m_select()
                .ld_column("F1", "A")
                .@as("MyColumn")
            .st_query()
            ;

            var visitor = new SQLVisitorBase();

            var res = visitor.Visit((SSyntaxNode)machine.Pop());

            var a = 10;
        }

        [Fact]
        public void UpdateTest()
        {
            var machine = new QueryMachine();
            machine.ct_query()
                .m_from()
                    .ld_table("table1")
                    .@as("t1")
                .m_where()
                    .ld_column("column1", "t1")
                    .ld_param("value1")
                    .eq()
                .m_set()
                    .ld_column("column1","t2")
                    .ld_column("column1", "t1")
                    .ld_column("column2", "t2")
                    .ld_column("column2", "t1")
                .m_update()
                    .ld_table("table2")
                    .@as("t2")
                .st_query();

            var visitor = new SQLVisitorBase();

            var res = visitor.Visit((SSyntaxNode)machine.Pop());

            var a = 10;
        }

        [Fact]
        public void InsertIntoSelectTest()
        {
            var machine = new QueryMachine();
            machine.ct_query()
                .m_from()
                    .ld_table("table1")
                    .@as("t1")
                .m_where()
                    .ld_column("column1", "t1")
                    .ld_param("value1")
                    .eq()
                .m_select()
                    .ld_column("column1", "t1")
                .m_insert()
                    .ld_table("table2")
                    .ld_column("column1")
                    
                .st_query();


            var visitor = new SQLVisitorBase();

            var res = visitor.Visit((SSyntaxNode)machine.Pop());

            var a = 10;



        }

        [Fact]
        public void InsertIntoValuesTest()
        {
            var machine = new QueryMachine();
            machine.ct_query()
                .m_values()
                    .ld_param("value1")
                    .ld_param("value2")
                    .ld_param("value3")
                .m_insert()
                    .ld_table("table2")
                    .ld_column("column1")
                    .ld_column("column2")
                    .ld_column("column3")
                    
                .st_query();


            var visitor = new SQLVisitorBase();

            var res = visitor.Visit((SSyntaxNode)machine.Pop());

            var a = 10;



        }
    }
}
