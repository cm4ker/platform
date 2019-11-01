using System.Linq;
using Xunit;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Core.Language.QueryLanguage;
using ZenPlatform.Core.Language.QueryLanguage.Model;
using ZenPlatform.QueryBuilder.Visitor;

namespace ZenPlatform.Core.Test.QLang
{
    public class QLangTest
    {
        private XCRoot conf = Factory.CreateExampleConfiguration();

        [Fact]
        public void QlangTest()
        {
            var q = new Language.QueryLanguage.Model.QLang(conf);

            q.begin_query();

            q.m_from();

            q.ld_component("Entity");
            q.ld_type("Invoice");
            q.alias("A");

            q.ld_component("Entity");
            q.ld_type("Store");
            q.alias("B");

            q.ld_name("A");
            q.ld_field("Store");

            q.ld_name("B");
            q.ld_field("Id");

            q.eq();

            q.on();

            q.join();

            q.m_select();

            q.ld_name("A");
            q.ld_field("Store");

            var result = (QField) q.top();

            Assert.Equal(2, result.GetRexpressionType().Count());

            q.st_query();

            var query = (QQuery) q.top();

            Assert.NotNull(query);
        }


        [Fact]
        public void QlangCaseTest()
        {
            var q = new Language.QueryLanguage.Model.QLang(conf);

            q.begin_query();

            q.m_from();

            q.ld_component("Entity");
            q.ld_type("Invoice");
            q.alias("A");

            q.m_select();

            q.ld_name("A");
            q.ld_field("Store");

            q.ld_param("Store");

            q.eq();

            q.ld_const(1);

            q.ld_const("Test");

            q.case_when();

            q.@case();

            var result = (QCase) q.top();

            Assert.NotNull(result);
            Assert.Single(result.Whens);

            Assert.NotNull(result.Whens[0]);
            Assert.NotNull(result.Whens[0].Then);
            Assert.NotNull(result.Whens[0].Else);
            Assert.NotNull(result.Whens[0].Then);

            Assert.Equal(2, result.GetRexpressionType().Count());

            q.st_query();

            var query = (QQuery) q.top();

            Assert.NotNull(query);
        }

        [Fact]
        public void NastedQueryTest()
        {
            var q = new Language.QueryLanguage.Model.QLang(conf);

            q.begin_query();

            q.m_from();

            q.ld_component("Entity");
            q.ld_type("Invoice");
            q.alias("A");

            //start nested query
            q.begin_query();
            q.m_from();

            q.ld_component("Entity");
            q.ld_type("Store");
            q.alias("B");

            q.m_select();

            q.ld_name("B");
            q.ld_field("Id");
            q.alias("NestedIdField");

            q.st_query();
            //store query on stack

            Assert.True(q.top() is QNestedQuery);

            q.alias("NestedQuery");

            q.ld_name("A");
            q.ld_field("Store");

            q.ld_name("NestedQuery");
            q.ld_field("NestedIdField");

            q.eq();

            q.on();

            q.join();

            q.m_select();

            q.ld_name("A");
            q.ld_field("Store");

            q.st_query();
            var query = q.top();
        }

        [Fact]
        public void WhereTest()
        {
            var q = new Language.QueryLanguage.Model.QLang(conf);
            q.begin_query();

            q.m_from();

            q.ld_component("Entity");
            q.ld_type("Invoice");
            q.alias("A");

            q.m_where();

            q.ld_name("A");
            q.ld_field("Id");

            q.ld_param("P_01");

            q.eq();
            q.m_select();

            Assert.True(q.top() is QWhere);
            q.ld_name("A");
            q.ld_field("Store");

            var result = (QField) q.top();

            Assert.Equal(2, result.GetRexpressionType().Count());

            q.st_query();

            var query = (QQuery) q.top();

            Assert.NotNull(query);
            Assert.NotNull(query.Where);
        }

        [Fact]
        public void Logic2RealTest()
        {
            var q = new Language.QueryLanguage.Model.QLang(conf);
            q.begin_query();

            q.m_from();

            q.ld_component("Entity");
            q.ld_type("Invoice");
            q.alias("A");

            q.m_where();

            q.ld_name("A");
            q.ld_field("Id");

            q.ld_param("P_01");

            q.eq();
            q.m_select();

            Assert.True(q.top() is QWhere);
            q.ld_name("A");
            q.ld_field("Store");
            q.alias("MyStore");

            var field = (QField) q.top();

            Assert.Equal(2, field.GetRexpressionType().Count());

            q.st_query();

            var query = (QQuery) q.top();

            SQLServerVisitor _visitor = new SQLServerVisitor();

            LogicToReal l2r = new LogicToReal();

            var syntaxTree = l2r.Build(query);

            var result = _visitor.Visit(syntaxTree);
        }
    }
}