using System.Linq;
using Xunit;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Core.Language.QueryLanguage.Model;

namespace ZenPlatform.Core.Test.QLang
{
    public class QLangTest
    {
        [Fact]
        public void QlangTest()
        {
            var conf = Factory.CreateExampleConfiguration();
            var q = new Language.QueryLanguage.Model.QLang(conf);

            q.begin_query();

            q.m_from();

            q.ld_component("Entity");
            q.ld_type("Invoice");
            q.alias("A");

            q.ld_component("Entity");
            q.ld_type("Store");
            q.alias("B");

            q.m_select();

            q.ld_name("A");
            q.ld_field("Store");

            var result = (QField) q.pop();

            Assert.Equal(1, result.GetRexpressionType().Count());
        }
    }
}