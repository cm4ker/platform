using System;
using Aquila.SyntaxGenerator.BoundTree;
using Aquila.SyntaxGenerator.Compiler;
using Aquila.SyntaxGenerator.Qlang;
using Aquila.SyntaxGenerator.QLang;
using Aquila.SyntaxGenerator.SQL;

namespace Aquila.SyntaxGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "compiler")
                AstSyntaxGenerator.Main(args[1..]);
            else if (args[0] == "sql")
                SQLSyntaxGenerator.Main(args[1..]);
            else if (args[0] == "qlang")
                QLangSyntaxGenerator.Main(args[1..]);
            else if (args[0] == "bound_tree")
                BoundTreeSyntaxGenerator.Main(args[1..]);
            else
                throw new Exception("No supported: use compiler | sql | qlang arguments before");
        }
    }
}