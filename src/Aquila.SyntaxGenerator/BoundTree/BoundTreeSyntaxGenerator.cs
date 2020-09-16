﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aquila.SyntaxGenerator.BoundTree
{
    public static class BoundTreeSyntaxGenerator
    {
        private static SyntaxToken publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
        private static SyntaxToken partialToken = SyntaxFactory.Token(SyntaxKind.PartialKeyword);
        private static SyntaxToken staticToken = SyntaxFactory.Token(SyntaxKind.StaticKeyword);

        private static string ns_root = "Aquila.CodeAnalysis.Semantics";
        private static string VisitorClassName = "AquilaOperationVisitor";
        private static string MSVisitorClassName = "OperationVisitor";
        private static string AquilaVisitorClassName = "AquilaOperationVisitor";
        private static string NameBase = "Operation";
        private static string Suffix = "Bound";

        private static ClassDeclarationSyntax astTreeBaseCls = SyntaxFactory.ClassDeclaration($"{VisitorClassName}<T>")
            .AddModifiers(publicToken)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword))
            .AddModifiers(partialToken);

        private static ClassDeclarationSyntax astTreeBaseCls2 = SyntaxFactory.ClassDeclaration(VisitorClassName)
            .AddModifiers(publicToken)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword))
            .AddModifiers(partialToken);


        public static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine($"Invalid using. Use {nameof(Aquila.SyntaxGenerator)} [PathToXmlScheme]");

            using (TextReader tr = new StreamReader(args[0]))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                var root = (Config) xs.Deserialize(tr);

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("/// <auto-generated />\n\n");

                var ns_definitions = $"{ns_root}";

                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using Aquila.CodeAnalysis.FlowAnalysis;");
                sb.AppendLine("using System.Collections.Immutable;");
                sb.AppendLine("using Aquila.Syntax.Text;");
                sb.AppendLine("using Aquila.CodeAnalysis.Semantics.Graph;");
                sb.AppendLine("using Aquila.CodeAnalysis.Symbols;");
                sb.AppendLine("using Aquila.Syntax.Ast;");
                sb.AppendLine("using Aquila.Syntax.Syntax;");
                sb.AppendLine("using Microsoft.CodeAnalysis;");
                sb.AppendLine("using Microsoft.CodeAnalysis.Operations;");

                foreach (var syntax in root.Syntaxes)
                {
                    sb.AppendLine($"namespace {ns_definitions + (syntax.NS != null ? "." : "") + syntax.NS} {{");
                    GenerateClass(sb, syntax, new List<CompilerSyntax>());
                    sb.AppendLine("}");
                }

                GenerateVisitor(sb, root.Syntaxes);


                if (args.Length == 2)
                {
                    using (var sw = new StreamWriter(args[1]))
                    {
                        var tree = SyntaxFactory.ParseSyntaxTree(sb.ToString());
                        sw.WriteLine(tree.GetRoot().NormalizeWhitespace());
                    }
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }

        private static void GenerateVisitor(StringBuilder sb, List<CompilerSyntax> syntaxes)
        {
            sb.AppendLine("namespace Aquila.CodeAnalysis.Semantics {");

            sb.AppendLine("using Aquila.CodeAnalysis.Semantics.TypeRef;");

            sb.AppendLine("abstract partial class AquilaOperationVisitor<TResult>{");

            foreach (var syntax in syntaxes)
            {
                sb.AppendLine(
                    $"public virtual TResult Visit{syntax.Name}({Suffix}{syntax.Name} x) => VisitDefault(x);");
            }

            sb.AppendLine("}");
            sb.AppendLine("}");
        }

        private static void GetVisitorMethod(StringBuilder sb, CompilerSyntax syntax)
        {
            sb.AppendLine(
                $"public override TRes Accept<TArg,TRes>({MSVisitorClassName}<TArg,TRes> visitor, TArg argument){{");
            sb.AppendLine($"TRes res = default; AcceptImpl(visitor, argument, ref res); return res;");
            sb.AppendLine("}");
        }

        private static void GetVisitorMethod2(StringBuilder sb, CompilerSyntax syntax)
        {
            sb.AppendLine(
                $"public override void Accept({MSVisitorClassName} visitor){{");
            sb.AppendLine($"AcceptImpl(visitor);");
            sb.AppendLine("}");
        }

        private static void GetVisitorMethodAquila(StringBuilder sb, CompilerSyntax syntax)
        {
            sb.AppendLine(
                $"public override TResult Accept<TResult>({AquilaVisitorClassName}<TResult> visitor){{");
            sb.AppendLine($"return visitor.Visit{syntax.Name}(this);");
            sb.AppendLine("}");
        }

        private static void UpdateMethod(StringBuilder sb, CompilerSyntax syntax)
        {
            if (syntax.Arguments.Any(x => x.IsInternal))
                sb.Append("internal ");
            else
                sb.Append("public ");
            
            sb.AppendLine(
                $"{Suffix}{syntax.Name} Update(");

            var args = syntax.Arguments.Where(x => x.IsUpdatable).ToList();

            var isTwoOrMore = false;

            foreach (var arg in args)
            {
                if (arg is SyntaxArgumentSingle s && !string.IsNullOrEmpty(s.PassBaseConst))
                    continue;

                if (isTwoOrMore)
                    sb.Append(",");

                sb.Append($"{arg.Type} {arg.Name.ToCamelCase()}");

                isTwoOrMore = true;
            }

            sb.Append(")");

            sb.AppendLine("{");


            sb.AppendLine("if (");


            var first = true;
            foreach (var arg in args)
            {
                if (!first)
                    sb.Append("&&");

                sb.AppendLine($"_{arg.Name.ToCamelCase()} == {arg.Name.ToCamelCase()}");

                first = false;
            }

            sb.Append(")");

            sb.AppendLine("return this;");

            sb.AppendLine($"return new {Suffix}{syntax.Name}(");

            isTwoOrMore = false;
            foreach (var arg in args)
            {
                if (isTwoOrMore)
                    sb.Append(",");

                sb.Append($"{arg.Name.ToCamelCase()}");

                isTwoOrMore = true;
            }

            sb.Append(");");


            sb.AppendLine("}");
        }


        private static void GenerateClass(StringBuilder sb, CompilerSyntax syntax, List<CompilerSyntax> baseList)
        {
            sb.AppendLine(
                $"{(syntax.IsAbstract ? "abstract" : "")} partial class {Suffix}{syntax.Name} : {Suffix}{syntax.Base ?? NameBase} {(!string.IsNullOrEmpty(syntax.Interface) ? $", {syntax.Interface}" : "")}");
            sb.AppendLine("{");

            //private fields
            foreach (var arg in syntax.Arguments)
            {
                if (arg is SyntaxArgumentSingle s && !string.IsNullOrEmpty(s.PassBaseConst))
                    continue;

                sb.AppendLine($"private {arg.Type} _{arg.Name.ToCamelCase()};");
            }

            //Constructor
            if (syntax.Arguments.Any(x => x.IsInternal))
                sb.Append("internal ");
            else
                sb.Append("public ");

            sb.AppendLine($"{Suffix}{syntax.Name}(");

            var isMoreThanOne = false;
            foreach (var arg in syntax.Arguments)
            {
                if (arg is SyntaxArgumentSingle s && !string.IsNullOrEmpty(s.PassBaseConst))
                    continue;

                if (isMoreThanOne)
                    sb.Append(",");

                sb.Append($"{arg.Type} {arg.Name.ToCamelCase()}");

                //default value
                if (arg is SyntaxArgumentSingle sas && !string.IsNullOrEmpty(sas.Default))
                    sb.Append($" = {sas.Default}");

                isMoreThanOne = true;
            }

            sb.Append(")");

            //Pass arguments to base class
            if (syntax.Arguments.Any(x => x.PassBase))
            {
                sb.Append(" : base(");

                isMoreThanOne = false;
                foreach (var arg in syntax.Arguments)
                {
                    if (arg.PassBase)
                    {
                        if (isMoreThanOne)
                        {
                            sb.Append(", ");
                        }

                        if (arg is SyntaxArgumentSingle s && !string.IsNullOrEmpty(s.PassBaseConst))
                            sb.Append(s.PassBaseConst);
                        else
                            sb.Append(arg.Name.ToCamelCase());

                        isMoreThanOne = true;
                    }
                }

                sb.Append(")");
            }

            sb.Append("{");

            foreach (var arg in syntax.Arguments)
            {
                if (arg is SyntaxArgumentSingle s && !string.IsNullOrEmpty(s.PassBaseConst))
                    continue;

                if (!arg.OnlyArgument)
                    sb.Append($"_{arg.Name.ToCamelCase()} = {arg.Name.ToCamelCase()};");
            }

            //call OnCreateImpl
            sb.AppendLine("OnCreateImpl(");

            isMoreThanOne = false;
            foreach (var arg in syntax.Arguments)
            {
                if (arg is SyntaxArgumentSingle s && !string.IsNullOrEmpty(s.PassBaseConst))
                    continue;

                if (isMoreThanOne)
                    sb.Append(",");

                sb.Append($"{arg.Name.ToCamelCase()}");


                isMoreThanOne = true;
            }

            sb.Append(");");

            sb.Append("}");

            //partial OnCreate()
            GenerateOnCreatePartialMethod(sb, syntax);

            foreach (var arg in syntax.Arguments)
            {
                if (arg is SyntaxArgumentSingle s && !string.IsNullOrEmpty(s.PassBaseConst))
                    continue;

                if (!arg.OnlyArgument && !arg.OnlyPrivate)
                {
                    if (arg.IsInternal)
                        sb.Append("internal");
                    else
                        sb.Append("public");

                    sb.AppendLine($" {arg.Type} {arg.Name} {{get {{ return _{arg.Name.ToCamelCase()};}}}}");
                }
            }

            if (!string.IsNullOrEmpty(syntax.OperationKind))
                sb.AppendLine($"public override OperationKind Kind => OperationKind.{syntax.OperationKind};");

            sb.AppendLine(
                "partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);");
            sb.AppendLine("partial void AcceptImpl(OperationVisitor visitor);");
            GetVisitorMethod(sb, syntax);
            GetVisitorMethod2(sb, syntax);
            GetVisitorMethodAquila(sb, syntax);

            if (syntax.Arguments.Any(x => x.IsUpdatable) && !syntax.IsAbstract)
                UpdateMethod(sb, syntax);

            sb.AppendLine("}");
        }

        private static void GenerateOnCreatePartialMethod(StringBuilder sb, CompilerSyntax syntax)
        {
            sb.AppendLine("partial void OnCreateImpl(");

            bool isMoreThanOne = false;
            foreach (var arg in syntax.Arguments)
            {
                if (arg is SyntaxArgumentSingle s && !string.IsNullOrEmpty(s.PassBaseConst))
                    continue;

                if (isMoreThanOne)
                    sb.Append(",");

                sb.Append($"{arg.Type} {arg.Name.ToCamelCase()}");

                isMoreThanOne = true;
            }

            sb.Append(");");
        }
    }
}