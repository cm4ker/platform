using System;
using System.IO;
using Antlr4.Runtime;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.Data;
using Aquila.Metadata;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;
using Aquila.Runtime;

namespace Aquila.UIBuilder
{
    public class Interpreter
    {
        private readonly DatabaseRuntimeContext _drContext;
        private readonly EntityMetadataCollection _mdCollection;
        private QLang _m;

        private class PrinterWalker : QLangWalker
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

                _output.Write(new String(' ', Depth * 4) + visitable.ToString());

                switch (visitable)
                {
                    case QField:
                        _output.Write($"(Name : {visitable.GetDbName()})");
                        break;
                    case QDataSource:
                        _output.Write($"(DS : {visitable.GetDbName()})");
                        break;
                    case QFromItem:
                        _output.Write($"(JOIN : {visitable.GetDbName()})");
                        break;
                }

                _output.WriteLine();

                base.DefaultVisit(visitable);
            }
        }

        public Interpreter(DatabaseRuntimeContext drContext)
        {
            _drContext = drContext;
            _mdCollection = TestMetadata.GetTestMetadata();

            _m = new QLang();
        }

        public (string Output1, string Output2) RunQuery(string sql)
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
                string sqlString = "";

                try
                {
                    var pwalker = new PhysicalNameWalker( _drContext);
                    pwalker.Visit(_m.top() as QLangElement);

                    var walker = new PrinterWalker(output);
                    walker.Visit(_m.top() as QLangElement);

                    var realWalker = new RealWalker(_drContext);
                    realWalker.Visit(_m.top() as QLangElement);


                    var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
                    sqlString = new MsSqlBuilder().Visit(syntax);
                }
                catch (Exception ex)
                {
                    sqlString = $"MSG: {ex.Message}\nST: {ex.StackTrace}";
                }


                return (output.ToString(), sqlString);
            }
            catch (Exception ex)
            {
                return ($"Runtime error: {ex.Message}\nST: {ex.StackTrace}", "");
            }
        }

        public (string Output1, string Output2) Run(string code)
        {
            try
            {
                _m.reset();

                StringReader sr = new StringReader(code);

                var cmd = sr.ReadLine();
                while (cmd != null)
                {
                    try
                    {
                        RunCommand(cmd);
                    }
                    catch (Exception ex)
                    {
                        return ($"Command: {cmd}\nMessage: {ex.Message}\nST:{ex.StackTrace}", "");
                    }

                    cmd = sr.ReadLine();
                }

                var output = new StringWriter();
                string sqlString = "";

                try
                {
                    var pwalker = new PhysicalNameWalker( _drContext);
                    pwalker.Visit(_m.top() as QLangElement);

                    var walker = new PrinterWalker(output);
                    walker.Visit(_m.top() as QLangElement);

                    var realWalker = new RealWalker(_drContext);
                    realWalker.Visit(_m.top() as QLangElement);


                    var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
                    sqlString = new MsSqlBuilder().Visit(syntax);
                }
                catch (Exception ex)
                {
                    sqlString = $"MSG: {ex.Message}\nST: {ex.StackTrace}";
                }


                return (output.ToString(), sqlString);
            }
            catch (Exception ex)
            {
                return ($"Runtime error: {ex.Message}\nST: {ex.StackTrace}", "");
            }
        }

        private void RunCommand(string cmd)
        {
            var args = cmd.Split(" ");

            var mName = args[0];

            if (args.Length > 1)
                typeof(QLang).GetMethod(mName)?.Invoke(_m, args[1..]);
            else
                typeof(QLang).GetMethod(mName)?.Invoke(_m, null);
        }
    }
}