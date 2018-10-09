using System.Security.Cryptography.X509Certificates;
using Xunit;
using ZenPlatform.Core.Language.QueryLanguage;
using ZenPlatform.Tests.Common;

namespace ZenPlatform.Tests.Core.LTWriter
{
    public class LTWriterTests
    {
        [Fact]
        public void SimpleWriter()
        {
            var conf = Factory.GetExampleConfigutaion();

            ZqlLogicalTreeWriter writer = new ZqlLogicalTreeWriter(conf);
            writer.WriteQuery();
            writer.WriteSource("Document", "TestEntity", "a");
            writer.WriteObjectField("СоставнойТип", "some_NotASimpleAlias", "a");
            writer.WriteCloseQuery();
            var res = writer.Result;
        }
    }
}