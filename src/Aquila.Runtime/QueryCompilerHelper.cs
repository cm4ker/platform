using System;
using Antlr4.Runtime;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;
using Aquila.Runtime;

namespace Aquila.Core
{
    public class QueryCompilerHelper
    {
        public static (string sql, QQueryList logicalTree) Compile(AqContext context, string sql)
        {
            var drc = context.DataRuntimeContext;
            //need compile sql expression!
            var qLangMachine = new QLang(drc.Metadata.GetMetadata());

            AntlrInputStream inputStream = new AntlrInputStream(sql);
            ZSqlGrammarLexer speakLexer = new ZSqlGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            ZSqlGrammarParser parser = new ZSqlGrammarParser(commonTokenStream);
            ZSqlGrammarVisitor visitor = new ZSqlGrammarVisitor(qLangMachine);

            visitor.Visit(parser.parse());

            string sqlString = "";

            var logicalTree = qLangMachine.top() as QQueryList ??
                              throw new Exception("Query stack machine after parsing MUST return the QueryList");

            //Create aliases for tree
            var pwalker = new PhysicalNameWalker(drc);
            pwalker.Visit(logicalTree);

            //Create query
            var realWalker = new RealWalker(drc);
            realWalker.Visit(logicalTree);

            //TODO: encapsulate builder into the context and let the platform choice
            var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
            return (new MsSqlBuilder().Visit(syntax), logicalTree);
        }
    }
}