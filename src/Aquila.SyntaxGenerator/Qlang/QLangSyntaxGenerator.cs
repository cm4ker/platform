﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Aquila.SyntaxGenerator.QLang;

namespace Aquila.SyntaxGenerator.QLang
{
    public static class QLangSyntaxGenerator
    {
        private static SyntaxToken publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
        private static SyntaxToken partialToken = SyntaxFactory.Token(SyntaxKind.PartialKeyword);
        private static SyntaxToken staticToken = SyntaxFactory.Token(SyntaxKind.StaticKeyword);

        private static ClassDeclarationSyntax astTreeBaseCls = SyntaxFactory.ClassDeclaration("QLangVisitorBase<T>")
            .AddModifiers(publicToken)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword))
            .AddModifiers(partialToken);

        private static ClassDeclarationSyntax astTreeBaseCls2 = SyntaxFactory.ClassDeclaration("QLangVisitorBase")
            .AddModifiers(publicToken)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword))
            .AddModifiers(partialToken);

        private static string ns_root = "Aquila.Core.Querying";
        private static string VisitorClassName = "QLangVisitorBase";
        private static string NameBase = "QLangElement";

        public static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine($"Invalid using. Use {nameof(Aquila.SyntaxGenerator)} [PathToXmlScheme]");

            using (TextReader tr = new StreamReader(args[0]))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                var root = (Config)xs.Deserialize(tr);

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("/// <auto-generated />\n\n");

                var ns_definitions = $"{ns_root}.Model";

                var unit = SyntaxFactory.CompilationUnit()
                    .AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Immutable")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Aquila.Metadata")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName($"{ns_definitions}"))
                    );


                foreach (var syntax in root.Syntaxes)
                {
                    var ns = SyntaxFactory.NamespaceDeclaration(
                        SyntaxFactory.ParseName(ns_definitions + (syntax.NS != null ? "." : "") + syntax.NS));

                    MemberDeclarationSyntax cls;

                    if (syntax.IsList)
                        cls = GenerateListClass(syntax);
                    else
                    {
                        IEnumerable<QLangSyntax> getAllBaseClasses(QLangSyntax syntax)
                        {
                            var baseSyntax = root.Syntaxes.FirstOrDefault(x => x.Name == syntax.Base);

                            while (baseSyntax != null)
                            {
                                yield return baseSyntax;

                                baseSyntax = root.Syntaxes.FirstOrDefault(x => x.Name == baseSyntax.Base);
                            }
                        }


                        cls = GenerateClass(syntax, getAllBaseClasses(syntax).ToList());
                    }

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

        private static List<FieldDeclarationSyntax> GetPrivateFieldsSyntax(QLangSyntax syntax)
        {
            var result = new List<FieldDeclarationSyntax>();

            foreach (var arg in syntax.Arguments)
            {
                if (arg.OnlyArgument)
                    continue;

                var fieldName = $"{arg.Name.ToCamelCase()}";
                result.Add((FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                    $"{((arg.IsProtected) ? "protected" : "private")} {arg.Type} {fieldName};"));
            }

            return result;
        }

        private static MethodDeclarationSyntax GetChildMethod(QLangSyntax syntax)
        {
            var sb = new StringBuilder($"public override IEnumerable<{NameBase}> GetChildren(){{");
            foreach (var arg in syntax.Arguments)
            {
                if (!arg.DenyChildrenFill)
                    sb.Append($"yield return this.{arg.Name.ToCamelCase()};");
            }

            if (!string.IsNullOrEmpty(syntax.Base))
                sb.Append($"foreach(var item in base.GetChildren()){{ yield return item; }}");

            sb.Append($"yield break;");
            sb.Append("}");

            return (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(sb.ToString());
        }

        private static MemberDeclarationSyntax GetAddMethod(QLangSyntax syntax)
        {
            var method = @$"public override {syntax.Name} Add({NameBase} element)
        {{
            var item = element as {syntax.Base};
            if (item == null)
                throw new Exception(""Element is not {syntax.Base}"");

            return new {syntax.Name}(Elements.Add(item));
        }}";

            return (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(method);
        }

        private static MemberDeclarationSyntax GetVisitorMethod(QLangSyntax syntax)
        {
            var sb = new StringBuilder();
            sb.Append(@$"public override T Accept<T>({VisitorClassName}<T> visitor){{");

            if (!syntax.IsAbstract)
            {
                sb.Append($"return visitor.Visit{syntax.Name}(this);");

                var visitorBaseMethod = (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                    $"public virtual T Visit{syntax.Name}({syntax.Name} arg) {{ return DefaultVisit(arg); }}");

                astTreeBaseCls = astTreeBaseCls.AddMembers(visitorBaseMethod);
            }
            else
            {
                sb.Append($"throw new NotImplementedException();");
            }

            sb.Append("}");

            return (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(sb.ToString());
        }

        private static MemberDeclarationSyntax GetVisitorMethod2(QLangSyntax syntax)
        {
            var visitor =
                (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                    $"public override void Accept({VisitorClassName} visitor){{}}");

            if (!syntax.IsAbstract)
            {
                visitor = visitor.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"visitor.Visit{syntax.Name}(this);"));

                var visitorBaseMethod = (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                    $"public virtual void Visit{syntax.Name}({syntax.Name} arg) {{ DefaultVisit(arg); }}");

                astTreeBaseCls2 = astTreeBaseCls2.AddMembers(visitorBaseMethod);
            }
            else
                visitor = visitor.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"throw new NotImplementedException();"));


            return visitor;
        }

        private static MemberDeclarationSyntax GenerateListClass(QLangSyntax syntax)
        {
            List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

            var ctor = (ConstructorDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                $"public {syntax.Name}(ImmutableArray<{syntax.Base}> elements) : base(elements){{}}");


            members.Add(
                SyntaxFactory.ParseMemberDeclaration(
                    $"public static {syntax.Name} Empty => new {syntax.Name}(ImmutableArray<{syntax.Base}>.Empty);"));
            members.Add(ctor);
            members.Add(GetAddMethod(syntax));
            members.Add(GetVisitorMethod(syntax));
            members.Add(GetVisitorMethod2(syntax));

            var record =
                (ClassDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                    $"public class {syntax.Name} : QLangCollection<{syntax.Base}>{{}}");

            return record.AddMembers(members.ToArray());
        }

        private static MemberDeclarationSyntax GenerateClass(QLangSyntax syntax, List<QLangSyntax> baseList)
        {
            List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

            var constructor = (ConstructorDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                $"public {syntax.Name}() : base(){{}}");

            var initializer = constructor.Initializer;

            //NOTE: handle ImplicitPassInChildren
            foreach (var baseSyntax in baseList)
            {
                foreach (var argument in baseSyntax.Arguments)
                {
                    if (argument.ImplicitPassInChildren)
                    {
                        var parameterSyntax = SyntaxFactory
                            .Parameter(SyntaxFactory.Identifier(argument.Name.ToCamelCase()))
                            .WithType(SyntaxFactory.ParseName(argument.Type));

                        if (argument is SyntaxArgumentSingle sas && sas.Default != null)
                            parameterSyntax = parameterSyntax.WithDefault(
                                SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(sas.Default)));


                        initializer = initializer.AddArgumentListArguments(
                            SyntaxFactory.Argument(SyntaxFactory.ParseName(argument.Name.ToCamelCase())));


                        constructor = constructor.AddParameterListParameters(parameterSyntax);
                    }
                }
            }

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

                    members.Add(SyntaxFactory.ParseMemberDeclaration(
                        $"public {argument.Type} {argument.Name} {{get => this.{argument.Name.ToCamelCase()}; init => this.{argument.Name.ToCamelCase()} = value;}}"));

                    if (!argument.DenyChildrenFill)
                    {
                        StatementSyntax fillStmt =
                            SyntaxFactory.ParseStatement(
                                $"this.{argument.Name.ToCamelCase()} = {argument.Name.ToCamelCase()};");

                        // SyntaxFactory.ParseStatement(
                        //     $"this.Attach({slot}, ({NameBase}){argument.Name.ToCamelCase()});");

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
            members.Add(GetChildMethod(syntax));
            GetPrivateFieldsSyntax(syntax).ForEach(members.Add);

            var cls = (ClassDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration($@"

public class {syntax.Name} : {(string.IsNullOrEmpty(syntax.Base) ? NameBase : syntax.Base)}
{{
    
}}
");

            cls = cls
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