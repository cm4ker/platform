using System;
using System.IO;
using System.Linq;
using System.Text;
using MoreLinq.Extensions;
using SharpGen.Runtime.Win32;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Core.Querying;
using ZenPlatform.Core.Querying.Model;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;

namespace ZenPlatform.UIBuilder
{
    public class Interpreter
    {
        private QLang _m;

        private class CustomWalker : QLangWalker
        {
            private readonly StringWriter _output;

            public CustomWalker(StringWriter output)
            {
                _output = output;
            }

            public override object Visit(QItem visitable)
            {
                _output.WriteLine(new String(' ', Depth * 4) + visitable.ToString());

                return base.Visit(visitable);
            }
        }

        public Interpreter()
        {
            _m = new QLang(Factory.CreateExampleConfiguration());
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
                    var walker = new CustomWalker(output);
                    walker.Visit(_m.top() as QItem);
                    var realWalker = new RealWalker();
                    realWalker.Visit(_m.top() as QItem);

                    var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
                    sqlString = new SQLVisitorBase().Visit(syntax);
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