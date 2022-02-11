using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Aquila.QueryBuilder;
using Xunit;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;

namespace Aquila.Runtime.Tests
{
    public class QueryMachineTest
    {
        private string CompileMsSql(QueryMachine m)
        {
            var syntax = m.peek();
            var visitor = new MsSqlBuilder();
            return visitor.Visit((SSyntaxNode)m.pop());
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

            Assert.Equal("SELECT Column\nFROM\nTable\n", CompileMsSql(machine));
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

            Assert.Equal("SELECT Column\nFROM\nTable\nJOIN Table2 ON 1 = 1\n", CompileMsSql(machine));
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

            Assert.Equal("SELECT CASE WHEN  1 = 1 THEN 2 \n  END,\nColumn\nFROM\nTable\n", CompileMsSql(machine));
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
                .bg_query()
                .m_select()
                .ld_param("param3")
                .ld_param("param2")
                .add()
                .@as("Summ")
                .st_query()
                .@as("subQuery")
                .ld_column("Summ", "subQuery")
                .ld_column("F2", "A")
                .eq()
                .left_join()
                .ld_table("T2")
                .@as("B")
                .ld_column("F1", "B")
                .ld_column("F1", "A")
                .eq()
                .join()
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

            var visitor = new MsSqlBuilder();

            var actual = visitor.Visit((SSyntaxNode)machine.pop());


            var expected =
                "SELECT A.F1 as MyColumn\nFROM\nT1 as A\nJOIN (SELECT @param3 + @param2 as Summ\n) as subQuery ON subQuery.Summ = A.F2\nJOIN T2 as B ON B.F1 = A.F1\nWHERE\n(A.F1 = B.F1 AND A.F2 = B.F2)\nGROUP BY\nA.F1\nHAVING\n@param4 > sum(A.F1)\n";

            Assert.Equal(expected, actual
            );
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
                .ld_column("column2", "t2")
                .ld_column("column2", "t1")
                .assign()
                .ld_column("column1", "t2")
                .ld_column("column1", "t1")
                .assign()
                .m_update()
                .ld_table("table2")
                .@as("t2")
                .st_query();

            var visitor = new MsSqlBuilder();

            var res = visitor.Visit((SSyntaxNode)machine.pop());

            Assert.Equal(res,
                "UPDATE table2 as t2\nSET t2.column2 = t1.column2, t2.column1 = t1.column1\nFROM\ntable1 as t1\nWHERE\nt1.column1 = @value1\n");
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


            var visitor = new MsSqlBuilder();

            var res = visitor.Visit((SSyntaxNode)machine.pop());
            Assert.Equal(res,
                res =
                    "INSERT INTO table2(column1)\nSELECT t1.column1\nFROM\ntable1 as t1\nWHERE\nt1.column1 = @value1\n");
        }

        [Fact]
        public void InsertIntoSelectTest2()
        {
            var machine = new QueryMachine();
            machine.bg_query()
                .m_from()
                .ld_table("source")
                .@as("t1")
                .m_select()
                .ld_column("*", "t1")
                .m_insert()
                .ld_table("destenation")
                //.ld_column("column1")
                .st_query();


            var visitor = new MsSqlBuilder();

            var res = visitor.Visit((SSyntaxNode)machine.pop());
            Assert.Equal(res,
                res =
                    "INSERT INTO destenation\nSELECT t1.*\nFROM\nsource as t1\n");
        }

        [Fact]
        public void InsertIntoValuesTest()
        {
            var machine = new QueryMachine();
            machine.bg_query()
                .m_values()
                .ld_param("value3")
                .ld_param("value2")
                .ld_param("value1")
                .m_insert()
                .ld_table("table2")
                .ld_column("column3")
                .ld_column("column2")
                .ld_column("column1")
                .st_query();


            var visitor = new MsSqlBuilder();

            var res = visitor.Visit((SSyntaxNode)machine.pop());

            Assert.Equal(res,
                res = "INSERT INTO table2(column3, column2, column1)\nVALUES\n(@value3, @value2, @value1)\n");
        }

        [Fact]
        public void QueryInSelectTest()
        {
            var machine = new QueryMachine();

            machine
                .bg_query()
                .m_from()
                .ld_table("Table")
                .m_select()
                .bg_query()
                .m_from()
                .ld_table("Table")
                .m_select()
                .ld_column("Column")
                .st_query()
                .ld_scalar()
                .ld_column("Column")
                .st_query();

            Assert.Equal("SELECT (SELECT Column\nFROM\nTable\n),\nColumn\nFROM\nTable\n", CompileMsSql(machine));
        }

        [Fact]
        public void QueryDeleteTest()
        {
            var machine = new QueryMachine();

            machine
                .bg_query()
                .m_from()
                .ld_table("Test")
                .@as("A")
                .m_where()
                .ld_column("Fld1")
                .ld_param("p0")
                .eq()
                .ld_column("Fld2")
                .ld_param("p1")
                .eq()
                .and()
                .m_delete()
                .ld_table("A")
                .st_query();

            Assert.Equal("DELETE A\nFROM\nTest as A\nWHERE\n(Fld1 = @p0 AND Fld2 = @p1)\n", CompileMsSql(machine));
        }
    }
}