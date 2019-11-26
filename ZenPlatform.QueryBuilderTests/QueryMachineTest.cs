using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Xunit;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;

namespace ZenPlatform.QueryBuilder.Tests
{
    public class QueryMachineTest
    {
        private string Compile(QueryMachine m)
        {
            var syntax = m.peek();
            var visitor = new SQLVisitorBase();

            return visitor.Visit((SSyntaxNode) m.pop());
        }

        [Fact]
        public void SimpleSelectTest()
        {
            var machine = new QueryMachine();

            machine
                .bg_query()
                .m_from()
                .ld_table("Table")
                .m_select()
                .ld_column("Column")
                .st_query();

            Assert.Equal("SELECT Column\nFROM\nTable\n", Compile(machine));
        }

        [Fact]
        public void SimpleJoinTest()
        {
            var machine = new QueryMachine();

            machine
                .bg_query()
                .m_from()
                .ld_table("Table")
                .ld_table("Table2")
                .ld_const(1)
                .ld_const(1)
                .eq()
                .@join()
                .m_select()
                .ld_column("Column")
                .st_query();

            Assert.Equal("SELECT Column\nFROM\nTable\nJOIN Table2 ON 1 = 1\n", Compile(machine));
        }

        [Fact]
        public void SimpleCaseTest()
        {
            var machine = new QueryMachine();

            machine
                .bg_query()
                .m_from()
                .ld_table("Table")
                .m_select()
                .ld_const(2)
                .ld_const(1)
                .ld_const(1)
                .eq()
                .when()
                .@case()
                
                .ld_column("Column")
                .st_query();

            Assert.Equal("SELECT Column,\nCASE WHEN  1 = 1 THEN 2 \n  END\nFROM\nTable\n", Compile(machine));
        }


        [Fact]
        public void SelectTest()
        {
            var machine = new QueryMachine();

            machine
                .bg_query()
                .m_from()
                .ld_table("T1")
                .@as("A")
                .ld_table("T2")
                .@as("B")
                .ld_column("F1", "B")
                .ld_column("F1", "A")
                .eq()
                .join()
                .bg_query()
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

            var res = visitor.Visit((SSyntaxNode) machine.pop());


            Assert.Equal(res, res = "SELECT A.F1 as MyColumn\nFROM\nT1 as A\nJOIN (SELECT @param3 as Summ\n) as subQuery ON A.F2 = subQuery.Summ\nJOIN T2 as B ON A.F1 = B.F1\nWHERE\nB.F2 = A.F2 and B.F1 = A.F1\nGROUP BY\nA.F1\nHAVING\n@param4 > sum(A.F1)\n");
        }

        [Fact]
        public void UpdateTest()
        {
            var machine = new QueryMachine();
            machine.bg_query()
                .m_from()
                .ld_table("table1")
                .@as("t1")
                .m_where()
                .ld_column("column1", "t1")
                .ld_param("value1")
                .eq()
                .m_set()
                .ld_column("column1", "t2")
                .ld_column("column1", "t1")
                .ld_column("column2", "t2")
                .ld_column("column2", "t1")
                .m_update()
                .ld_table("table2")
                .@as("t2")
                .st_query();

            var visitor = new SQLVisitorBase();

            var res = visitor.Visit((SSyntaxNode) machine.pop());

            Assert.Equal(res, res = "UPDATE table2 as t2\nSET t2.column2 = t1.column2, t2.column1 = t1.column1\nFROM\ntable1 as t1\nWHERE\n@value1 = t1.column1\n");
        }

        [Fact]
        public void InsertIntoSelectTest()
        {
            var machine = new QueryMachine();
            machine.bg_query()
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

            var res = visitor.Visit((SSyntaxNode) machine.pop());
            Assert.Equal(res, res = "INSERT INTO table2(column1)\nSELECT t1.column1\nFROM\ntable1 as t1\nWHERE\n@value1 = t1.column1\n");
        }

        [Fact]
        public void InsertIntoValuesTest()
        {
            var machine = new QueryMachine();
            machine.bg_query()
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

            var res = visitor.Visit((SSyntaxNode) machine.pop());

            Assert.Equal(res, res = "INSERT INTO table2(column3, column2, column1)\nVALUES\n(@value3, @value2, @value1)\n");
        }
    }
}