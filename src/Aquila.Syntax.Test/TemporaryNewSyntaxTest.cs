using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Aquila.Compiler.Test2
{
    public class TemporaryNewSyntaxTest
    {
        [Fact]
        public void Syntax2Test()
        {
            CSharpSyntaxTree.ParseText("public void test() {} ");
        }
    }
}