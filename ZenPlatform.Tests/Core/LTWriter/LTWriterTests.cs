using System.Security.Cryptography.X509Certificates;
using Xunit;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Core.Language.QueryLanguage;

namespace ZenPlatform.Tests.Core.LTWriter
{
    public class LTWriterTests
    {
        private XCRoot conf = Factory.CreateExampleConfiguration();

        public LTWriterTests()
        {
        }

        [Fact]
        public void SimpleWriter()
        {
            ZqlLogicalTreeWriter writer = new ZqlLogicalTreeWriter(conf);

            writer.WriteQuery();
            writer.WriteSource("Entity", "ТестоваяСущность", "a");
            writer.WriteSelect();
            writer.WriteObjectField("СоставнойТип", "some_NotASimpleAlias", "a");
            writer.WriteCloseQuery();

            var res = writer.Result;
        }

        [Fact]
        public void WhereTest()
        {
            var writer = new ZqlLogicalTreeWriter(conf);

            writer.WriteQuery();
            writer.WriteSource("Entity", "ТестоваяСущность", "a");

            writer.WriteWhere();

            writer.WriteEqualsOperator();
            writer.WriteObjectField("СоставнойТип", "", "a");
            writer.WriteObjectField("СоставнойТип", "", "a");
            writer.WriteEndExpression();

            writer.WriteSelect();
            writer.WriteObjectField("СоставнойТип", "some_NotASimpleAlias", "a");
            writer.WriteCloseQuery();

            var res = writer.Result;
        }

        [Fact]
        public void CaseTest()
        {
            ZqlLogicalTreeWriter writer = new ZqlLogicalTreeWriter(conf);

            writer.WriteQuery();
            writer.WriteSource("Entity", "Invoice", "a");
            writer.WriteSelect();
            writer.WriteCaseOperator("Aliase");

            writer.WriteEqualsOperator();
            writer.WriteObjectField("СоставнойТип", "some_NotASimpleAlias", "a");
            writer.WriteObjectField("СоставнойТип", "some_NotASimpleAlias", "a");
            writer.WriteEndExpression();

            writer.WriteObjectField("СоставнойТип", "some_NotASimpleAlias", "a");

            writer.WriteObjectField("СоставнойТип", "some_NotASimpleAlias", "a");

            writer.WriteEndExpression();

            writer.WriteCloseQuery();

            var res = writer.Result;
        }
    }
}