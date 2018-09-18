using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Xunit;
using ZenPlatform.Core.Language.QueryLanguage;

namespace ZenPlatform.Tests.Core
{
    public class QueryTests
    {
        [Fact]
        public void ParserTest()
        {
            var zsql = "SELECT i.f1, i.f2, i.f3 FROM Document.Invoice i";

            AntlrInputStream inputStream = new AntlrInputStream(zsql);
            ZSqlGrammarLexer speakLexer = new ZSqlGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            ZSqlGrammarParser speakParser = new ZSqlGrammarParser(commonTokenStream);
            ZSqlGrammarVisitor visitor = new ZSqlGrammarVisitor();

            visitor.Visit(speakParser.parse());
        }

    }
}
