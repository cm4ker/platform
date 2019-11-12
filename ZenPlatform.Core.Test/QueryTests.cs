using Antlr4.Runtime;
using Xunit;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Core.Quering;
using ZenPlatform.Core.Quering.QueryLanguage;

namespace ZenPlatform.Core.Test
{
    public class QueryTests
    {

        [Fact]
        public void ParserTest()
        {

            var conf = Factory.CreateExampleConfiguration();
            var context = new DataQueryConstructorContext();


            var zsql = "SELECT i.f1, i.f2, i.f3 FROM Document.Invoice i";

            AntlrInputStream inputStream = new AntlrInputStream(zsql);
            ZSqlGrammarLexer speakLexer = new ZSqlGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            ZSqlGrammarParser speakParser = new ZSqlGrammarParser(commonTokenStream);
            ZSqlGrammarVisitor visitor = new ZSqlGrammarVisitor(conf, context);

            visitor.Visit(speakParser.parse());
        }


        [Fact]
        public void LogicalTreeTest()
        {
            
        }
    }
}
