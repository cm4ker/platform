using System;
using Aquila.Syntax.Parser;

namespace Aquila.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var syntax = ParserHelper.ParseSyntax(@"
pub Sum(int x, int y)  
{
    return x + y;
}
");
            System.Console.WriteLine(syntax.GetType());
        }
    }
}