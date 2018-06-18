using System;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using ZenPlatform.QueryBuilder2.DML.Delete;
using ZenPlatform.QueryBuilder2.DML.From;
using ZenPlatform.QueryBuilder2.DML.Insert;
using ZenPlatform.QueryBuilder2.DML.Select;
using ZenPlatform.QueryBuilder2.DML.Update;

namespace ZenPlatform.QueryBuilder2
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
                .Where((f) => f.Field("field1"), "=", (f) => f.Field("field2"));

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
                .WithFieldAndValue(x => x.Field("One"), v => v.Parameter("p0"));

            var sb = new StringBuilder();
            c.Compile(q, sb);
            sb.Append("\n=============================\n");
            c.Compile(u, sb);
            sb.Append("\n=============================\n");
            c.Compile(d, sb);
            sb.Append("\n=============================\n");
            c.Compile(i, sb);


            Console.WriteLine(sb);
        }
    }
}