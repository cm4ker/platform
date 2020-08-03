﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aquila.SyntaxGenerator.Compiler
{
    public static class ComilerSyntaxGenerator
    {
        private static SyntaxToken publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
        private static SyntaxToken partialToken = SyntaxFactory.Token(SyntaxKind.PartialKeyword);
        private static SyntaxToken staticToken = SyntaxFactory.Token(SyntaxKind.StaticKeyword);

        private static ClassDeclarationSyntax astTreeBaseCls = SyntaxFactory.ClassDeclaration("AstVisitorBase<T>")
            .AddModifiers(publicToken)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword))
            .AddModifiers(partialToken);

        private static ClassDeclarationSyntax astTreeBaseCls2 = SyntaxFactory.ClassDeclaration("AstVisitorBase")
            .AddModifiers(publicToken)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword))
            .AddModifiers(partialToken);

        private static string ns_root = "Aquila.Syntax";
        private static string VisitorClassName = "AstVisitorBase";
        private static string NameBase = "LangElement";

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

                var ns_definitions = $"{ns_root}.Ast";

                var unit = SyntaxFactory.CompilationUnit()
                    .AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Aquila.Syntax.Text")),
                        // SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Aquila.Language.Ast")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName($"{ns_definitions}")),
                        SyntaxFactory.UsingDirective(
                            SyntaxFactory.ParseName($"{ns_definitions}.Expressions")),
                        SyntaxFactory.UsingDirective(
                            SyntaxFactory.ParseName($"{ns_definitions}.Statements")),
                        SyntaxFactory.UsingDirective(
                            SyntaxFactory.ParseName($"{ns_definitions}.Functions")) //,
                        // SyntaxFactory.UsingDirective(
                        //     SyntaxFactory.ParseName("Aquila.Language.Ast.Infrastructure")),
                        //SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Aquila.Language.Ast.Symbols"))
                    );


                foreach (var syntax in root.Syntaxes)
                {
                    var ns = SyntaxFactory.NamespaceDeclaration(
                        SyntaxFactory.ParseName(ns_definitions + (syntax.NS != null ? "." : "") + syntax.NS));

                    MemberDeclarationSyntax cls;

                    if (syntax.IsList)
                        cls = GenerateListClass(syntax);
                    else
                        cls = GenerateClass(syntax);

                    ns = ns.AddMembers(cls);
                    unit = unit.AddMembers(ns);

                    //Console.WriteLine(ns.NormalizeWhitespace());
                }

                var nsAst = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(ns_root));

                nsAst = nsAst.AddMembers(astTreeBaseCls);
                nsAst = nsAst.AddMembers(astTreeBaseCls2);

                unit = unit.AddMembers(nsAst);

                if (args.Length == 2)
                {
                    using (var sw = new StreamWriter(args[1]))
                    {
                        sw.WriteLine(unit.NormalizeWhitespace());
                    }
                }
                else
                {
                    Console.WriteLine(unit.NormalizeWhitespace());
                }
            }
        }

        private static MemberDeclarationSyntax GetVisitorMethod(CompilerSyntax syntax)
        {
            var visitor =
                (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                    $"public override T Accept<T>({VisitorClassName}<T> visitor){{}}");

            if (!syntax.IsAbstract)
            {
                visitor = visitor.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"return visitor.Visit{syntax.Name}(this);"));

                var visitorBaseMethod = (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                    $"public virtual T Visit{syntax.Name}({syntax.Name} arg) {{ return DefaultVisit(arg); }}");

                astTreeBaseCls = astTreeBaseCls.AddMembers(visitorBaseMethod);
            }
            else
                visitor = visitor.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"throw new NotImplementedException();"));


            return visitor;
        }

        private static MemberDeclarationSyntax GetVisitorMethod2(CompilerSyntax syntax)
        {
            var visitor =
                (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                    "public override void Accept(AstVisitorBase visitor){}");

            if (!syntax.IsAbstract)
            {
                visitor = visitor.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"visitor.Visit{syntax.Name}(this);"));

                var visitorBaseMethod = (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                    $"public virtual void Visit{syntax.Name}({syntax.Name} arg) {{ DefaultVisit(arg); }}");

                astTreeBaseCls2 = astTreeBaseCls2.AddMembers(visitorBaseMethod);
            }
            else
                visitor = visitor.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"throw new NotImplementedException();"));


            return visitor;
        }

        private static MemberDeclarationSyntax GenerateListClass(CompilerSyntax syntax)
        {
            List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

            var initializer = SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                SyntaxFactory.ArgumentList()
                    .AddArguments(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("Span.Empty"))));

            var constructor = SyntaxFactory.ConstructorDeclaration(syntax.Name)
                .WithBody(SyntaxFactory.Block())
                .WithModifiers(SyntaxTokenList.Create(publicToken))
                .WithInitializer(initializer);


            members.Add(
                SyntaxFactory.ParseMemberDeclaration($"public static {syntax.Name} Empty => new {syntax.Name}();"));
            members.Add(constructor);
            members.Add(GetVisitorMethod(syntax));
            members.Add(GetVisitorMethod2(syntax));

            return SyntaxFactory.ClassDeclaration(syntax.Name)
                .WithModifiers(SyntaxTokenList.Create(publicToken))
                .AddMembers(members.ToArray())
                .WithBaseList(SyntaxFactory.BaseList()
                    .AddTypes(SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName($"SyntaxCollectionNode<{syntax.Base}>"))));
        }


        private static MemberDeclarationSyntax GenerateClass(CompilerSyntax syntax)
        {
            List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();


            var constructor = (ConstructorDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                $"public {syntax.Name}(Span span, SyntaxKind syntaxKind) : base(span, syntaxKind){{}}");

            var initializer = constructor.Initializer;


            var slot = 0;

            foreach (var argument in syntax.Arguments)
            {
                var parameterSyntax = SyntaxFactory
                    .Parameter(SyntaxFactory.Identifier(argument.Name.ToCamelCase()))
                    .WithType(SyntaxFactory.ParseName(argument.Type));

                if (argument is SyntaxArgumentSingle sas && sas.Default != null)
                    parameterSyntax = parameterSyntax.WithDefault(
                        SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(sas.Default)));


                if (argument.PassBase)
                    initializer = initializer.AddArgumentListArguments(
                        SyntaxFactory.Argument(SyntaxFactory.ParseName(argument.Name.ToCamelCase())));


                constructor = constructor.AddParameterListParameters(parameterSyntax);


                if (!argument.OnlyArgument)
                {
                    var getter = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);

                    if (argument.DenyChildrenFill)
                        getter = getter.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                    else
                        getter = getter.AddBodyStatements(
                            SyntaxFactory.ParseStatement($"return ({argument.Type})this.Children[{slot}];"));

                    members.Add(SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(argument.Type),
                            argument.Name).AddModifiers(publicToken)
                        .WithAccessorList(SyntaxFactory.AccessorList()
                            .AddAccessors(getter)));


                    if (!argument.DenyChildrenFill)
                    {
                        StatementSyntax fillStmt =
                            SyntaxFactory.ParseStatement(
                                $"this.Attach({slot}, ({NameBase}){argument.Name.ToCamelCase()});");

                        constructor = constructor.AddBodyStatements(fillStmt);

                        slot++;
                    }
                    else
                    {
                        var ae = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.ParseName(argument.Name),
                            SyntaxFactory.ParseName(argument.Name.ToCamelCase()));

                        constructor = constructor.AddBodyStatements(SyntaxFactory.ExpressionStatement(ae));
                    }
                }
            }

            constructor = constructor.WithInitializer(initializer);


            members.Add(GetVisitorMethod(syntax));
            members.Add(GetVisitorMethod2(syntax));

            var cls = SyntaxFactory.ClassDeclaration(syntax.Name)
                .WithModifiers(SyntaxTokenList.Create(publicToken))
                .WithBaseList(SyntaxFactory.BaseList().AddTypes(SyntaxFactory
                    .SimpleBaseType(
                        SyntaxFactory.ParseTypeName(string.IsNullOrEmpty(syntax.Base) ? NameBase : syntax.Base))))
                .AddMembers(constructor)
                .AddMembers(members.ToArray());

            if (syntax.IsAbstract)
                cls = cls.AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));

            cls = cls.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            if (syntax.IsScoped)
            {
                cls = cls.AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IScoped")));

                cls = cls.AddMembers(SyntaxFactory.PropertyDeclaration(
                        SyntaxFactory.ParseTypeName("SymbolTable"),
                        "SymbolTable").AddModifiers(publicToken)
                    .WithAccessorList(SyntaxFactory.AccessorList()
                        .AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                        .AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));
            }

            if (syntax.IsSymbol)
            {
                cls = cls.AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IAstSymbol")));

                if (syntax.Arguments.All(x => x.Name != "Name") && !syntax.NotThrowExMembersNotFound)
                {
                    throw new Exception(
                        $"Syntax {syntax.Name} has Symbol attribute but hasn't Name argument! Please add the Name argument for the escape this exception or add `NotThrowExMembersNotFound` attribute");
                }
            }

            return cls;
        }
    }
}