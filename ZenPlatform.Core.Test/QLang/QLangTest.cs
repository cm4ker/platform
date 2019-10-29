using System.Linq;
using Xunit;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Core.Language.QueryLanguage.Model;

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

            Assert.True(q.top() is QNastedQuery);

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
    }
}