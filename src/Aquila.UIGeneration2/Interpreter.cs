using System;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.Data;
using Aquila.Metadata;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;
using Aquila.Runtime;
using Aquila.Runtime.Querying;

namespace Aquila.UIBuilder
{
    public class PrinterWalker : QLangWalker
    {
        private readonly StringWriter _output;

        public PrinterWalker(StringWriter output)
        {
            _output = output;
        }

        public override void DefaultVisit(QLangElement visitable)
        {
            if (visitable is null)
                return;

            if (string.IsNullOrEmpty(visitable.ToString()))
            {
                base.DefaultVisit(visitable);
                return;
            }

            _output.Write(new String(' ', Depth * 4) + visitable ?? "");

            switch (visitable)
            {
                case QField:
                    _output.Write($"(Name : {visitable.GetDbName() ?? ""})");
                    break;
                case QDataSource:
                    _output.Write($"(DS : {visitable.GetDbName() ?? ""})");
                    break;
                case QFromItem:
                    _output.Write($"(JOIN : {visitable.GetDbName() ?? ""})");
                    break;
            }


            if (visitable is QExpression e)
                _output.Write(
                    $"(Type : {e.GetExpressionType().Aggregate("", (s, type) => $"{s}|{type.Name}")})");

            _output.WriteLine();

            base.DefaultVisit(visitable);
        }
    }

    public class Interpreter
    {
        private readonly DatabaseRuntimeContext _drContext;
        private readonly MetadataProvider _mdProvider;
        private QLang _m;
        private readonly UserSecTable _ust;


        public Interpreter(DatabaseRuntimeContext drContext)
        {
            _drContext = drContext;
            _mdProvider = TestMetadata.GetTestMetadata();


            _ust = new UserSecTable();
            _ust.Init(_mdProvider.GetSecPolicies().ToList(), _mdProvider);

            _m = new QLang(_mdProvider);
        }

        public (string Output1, string Output2, string Translated) RunQuery(string sql)
        {
            try
            {
                _m.reset();

                AntlrInputStream inputStream = new AntlrInputStream(sql);
                ZSqlGrammarLexer speakLexer = new ZSqlGrammarLexer(inputStream);
                CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
                ZSqlGrammarParser parser = new ZSqlGrammarParser(commonTokenStream);
                ZSqlGrammarVisitor visitor = new ZSqlGrammarVisitor(_m);

                visitor.Visit(parser.parse());

                var output = new StringWriter();
                var translatedOutput = new StringWriter();
                string sqlString = "";

                try
                {
                    var element = _m.top() as QLangElement;
                    var t = new SecurityVisitor(_mdProvider, _ust);

                    var translated = t.Visit(element);
                    new PrinterWalker(translatedOutput).Visit(translated);
                    new PrinterWalker(output).Visit(element);

                    var pwalker = new PhysicalNameWalker(_drContext);
                    pwalker.Visit(translated);

                    var realWalker = new SelectionRealWalker(_drContext);
                    realWalker.Visit(translated);

                    var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
                    sqlString = new MsSqlBuilder().Visit(syntax);
                }
                catch (Exception ex)
                {
                    sqlString = $"MSG: {ex.Message}\nST: {ex.StackTrace}";
                }


                return (output.ToString(), sqlString, translatedOutput.ToString());
            }
            catch (Exception ex)
            {
                return ($"Runtime error: {ex.Message}\nST: {ex.StackTrace}", "", "Empty");
            }
        }
    }
}