using System;
using System.Text;
using Aquila.QueryBuilder.Common.SqlTokens;
using Aquila.QueryBuilder.DDL.CreateDatabase;
using Aquila.QueryBuilder.DDL.CreateTable;
using Aquila.QueryBuilder.DDL.Table;
using Aquila.QueryBuilder.DML.Delete;
using Aquila.QueryBuilder.DML.From;
using Aquila.QueryBuilder.DML.Insert;
using Aquila.QueryBuilder.DML.Select;
using Aquila.QueryBuilder.DML.Update;

namespace Aquila.QueryBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var q = new SelectQueryNode()
                .WithTop(10)
                .From("Table1", (t) => t.As("t1"))
                .Join(JoinType.Inner, "test", t => t.As("t2"),
                    o => o.On("t1", "FiledT1", "=", "t2", "FieldT2"))
                .Join(JoinType.Full,
                    (nastedQuery) =>
                    {
                        nastedQuery.Select("SomeField").From("TableNasted", t => t.As("tn"))
                            .Join(JoinType.Left, "NastedJoinTable", t => { t.As("njt"); },
                                j => j.On("tn", "fn", "<=", "njt", "fn"))
                            .Where(f => f.Field("f1"), "=", f => f.Parameter("Param"));
                    },
                    (n) => { n.As("Nasted"); }, (o) => o.On("t1", "FieldT1", "<>", "Nasted", "NastedField"))
                .Select(tableName: "someTable", fieldName: "field1", alias: "HeyYouAreNewField")
                .SelectRaw("CASE WHEN 1 = 1 THEN '1' ELSE '2' END")
                .Where((f) => f.Field("field1"), "=", (f) => f.Field("field2"))
                .Where(x=>x.Or(t=> new []{t.Condition(j=>j.Raw("test"), new EqualsToken(), o=>o.Raw("test2"))}));

            var c = new SqlServerCompiller();

            var u = new UpdateQueryNode()
                .Update("t")
                .Set(f => f.Field("t", "Field1"), v => v.Parameter("p0"))
                .From("TestTable", "t")
                .WhereLike(f => f.Field("t", "Field1"), "a%");

            var d = new DeleteQueryNode()
                .Delete("t")
                .From("TestTable", t => t.As("t").WithSchema("dbo"))
                .WhereLike(f => f.Field("t", "Field1"), "a%");

            var i = new InsertQueryNode()
                .InsertInto("dbo", "Test")
                .WithFieldAndValue(x => x.Field("One"), v => v.Parameter("p0"))
                .WithFieldAndValue(x => x.Field("Two"), f => f.Parameter("p1"));

            var cr = new CreateTableQueryNode("dbo", "someAlterTable")
                .WithColumn("Id", f => f.Int())
                .WithColumn("Name", f => f.Varchar(30).NotNull());

            var a = new AlterTableQueryNode("someAlterTable", t => t.WithSchema("dbo"));
            a.AddColumn("ТристаОтсосиУТракториста", f => f.Guid().NotNull());

            var createDb = new CreateDatabaseQueryNode("test Database");

            var sb = new StringBuilder();
            c.Compile(q, sb);
            sb.Append("\n=============================\n");
            c.Compile(u, sb);
            sb.Append("\n=============================\n");
            c.Compile(d, sb);
            sb.Append("\n=============================\n");
            c.Compile(i, sb);
            sb.Append("\n=============================\n");
            c.Compile(cr, sb);
            sb.Append("\n=============================\n");
            c.Compile(a, sb);
            sb.Append("\n=============================\n");
            c.Compile(createDb, sb);

            Console.WriteLine(sb);
            Console.ReadLine();
        }
    }

    public enum SqlDatabaseType
    {
        SqlServer,
        Postgres,
        MySql,
        Oracle
    }
}