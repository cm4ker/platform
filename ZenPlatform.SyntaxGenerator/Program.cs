using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZenPlatform.SyntaxGenerator.SQL;

namespace ZenPlatform.SyntaxGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            ComilerSyntaxGenerator.Main(args);
            //SQLSyntaxGenerator.Main(args);
        }
    }

}