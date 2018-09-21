﻿using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Xunit;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Core.Language.QueryLanguage;

namespace ZenPlatform.Tests.Core
{
    public class QueryTests
    {

        [Fact]
        public void ParserTest()
        {

            var conf = Common.Factory.GetExampleConfigutaion();
            var context = new DataQueryConstructorContext();


            var zsql = "SELECT i.f1, i.f2, i.f3 FROM Document.Invoice i";

            AntlrInputStream inputStream = new AntlrInputStream(zsql);
            ZSqlGrammarLexer speakLexer = new ZSqlGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            ZSqlGrammarParser speakParser = new ZSqlGrammarParser(commonTokenStream);
            ZSqlGrammarVisitor visitor = new ZSqlGrammarVisitor(conf, context);

            visitor.Visit(speakParser.parse());
        }

    }
}
