using System;
using ZenPlatform.SyntaxGenerator.Compiler;
using ZenPlatform.SyntaxGenerator.QLang;
using ZenPlatform.SyntaxGenerator.SQL;

namespace ZenPlatform.SyntaxGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "compiler")
                ComilerSyntaxGenerator.Main(args[1..]);
            if (args[0] == "sql")
                SQLSyntaxGenerator.Main(args[1..]);
            if (args[0] == "qlang")
                QLangSyntaxGenerator.Main(args[1..]);
            else
                throw new Exception("No supported: use compiler | sql | qlang arguments before");
        }
    }
}