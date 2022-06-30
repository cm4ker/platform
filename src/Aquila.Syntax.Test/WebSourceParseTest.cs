using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax.InternalSyntax;
using Aquila.CodeAnalysis.Web;
using Microsoft.CodeAnalysis.Text;
using Xunit;
using Xunit.Abstractions;

namespace Aquila.Compiler.Test2;

public class WebSourceParseTest
{
    private readonly ITestOutputHelper _output;

    public WebSourceParseTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TagParseTest()
    {
        Lexer a = new Lexer(SourceText.From(
            @"

<h1>
    @for(int i = 0; i < 5; i++)
    {
        <div>Hello world @i</div>
        
    }
</h1>
 

"), AquilaParseOptions.Default);
        while (true)
        {
            var token = a.Lex(LexerMode.ViewSyntax);
            
        }
    }
}