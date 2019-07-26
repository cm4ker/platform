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

namespace ZenPlatform.SyntaxGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine($"Invalid using. Use {nameof(ZenPlatform.SyntaxGenerator)} [PathToXmlScheme]");

            using (TextReader tr = new StreamReader(args[0]))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                var root = (Config) xs.Deserialize(tr);


                StringBuilder sb = new StringBuilder();


                sb.AppendLine("/// <auto-generated />\n\n");

                var unit = SyntaxFactory.CompilationUnit()
                    .AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ZenPlatform.Compiler.Contracts.Symbols")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ZenPlatform.Language.Ast")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ZenPlatform.Language.Ast.Definitions")),
                        SyntaxFactory.UsingDirective(
                            SyntaxFactory.ParseName("ZenPlatform.Language.Ast.Definitions.Functions")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ZenPlatform.Language.Ast.Infrastructure"))
                    );


                var rootNs = "ZenPlatform.Language.Ast.Definitions";

                foreach (var syntax in root.Syntaxes)
                {
                    var ns = SyntaxFactory.NamespaceDeclaration(
                        SyntaxFactory.ParseName(rootNs + (syntax.NS != null ? "." : "") + syntax.NS));

                    List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

                    var slot = SyntaxFactory.ParseStatement("var slot = 0;");

                    var publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);

                    var constructor = SyntaxFactory.ConstructorDeclaration(syntax.Name)
                        .WithParameterList(SyntaxFactory.ParameterList()
                            .AddParameters(SyntaxFactory.Parameter(
                                SyntaxFactory.Identifier("lineInfo")).WithType(SyntaxFactory.ParseName("ILineInfo"))))
                        .WithBody(SyntaxFactory.Block())
                        .WithModifiers(SyntaxTokenList.Create(publicToken));

                    var initializer = SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                        SyntaxFactory.ArgumentList()
                            .AddArguments(SyntaxFactory.Argument(SyntaxFactory.ParseName("lineInfo"))));


                    if (syntax.Arguments.Count > 0)
                    {
                        constructor = constructor.AddBodyStatements(slot);
                    }

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

                        var ae = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.ParseName(argument.Name),
                            SyntaxFactory.ParseName(argument.Name.ToCamelCase()));

                        constructor = constructor.AddBodyStatements(SyntaxFactory.ExpressionStatement(ae));

                        members.Add(SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(argument.Type),
                                argument.Name).AddModifiers(publicToken)
                            .WithAccessorList(SyntaxFactory.AccessorList()
                                .AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));

                        if (!argument.DenyChildrenFill)
                        {
                            StatementSyntax fillStmt;
                            // fill childrens
                            if (argument is SyntaxArgumentList)
                                fillStmt =
                                    SyntaxFactory.ParseStatement(
                                        $"foreach(var item in {argument.Name}) {{Childs.Add(item);}}");
                            else if (argument is SyntaxArgumentSingle)
                            {
                                fillStmt =
                                    SyntaxFactory.ParseStatement(
                                        $"Childs.Add({argument.Name});");
                            }
                            else
                            {
                                throw new Exception();
                            }

                            constructor = constructor.AddBodyStatements(fillStmt);
                        }
                    }

                    constructor = constructor.WithInitializer(initializer);
                    
                    var visitor =
                        (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                            "public override T Accept<T>(AstVisitorBase<T> visitor){}");

                    if (!syntax.IsAbstract)
                        visitor = visitor.AddBodyStatements(
                            SyntaxFactory.ParseStatement($"return visitor.Visit{syntax.Name}(this);"));
                    else
                        visitor = visitor.AddBodyStatements(
                            SyntaxFactory.ParseStatement($"throw new NotImplementedException();"));
                    members.Add(visitor);

                    var cls = SyntaxFactory.ClassDeclaration(syntax.Name)
                        .WithModifiers(SyntaxTokenList.Create(publicToken))
                        .WithBaseList(SyntaxFactory.BaseList().AddTypes(SyntaxFactory
                            .SimpleBaseType(SyntaxFactory.ParseTypeName(syntax.Base))))
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

                    ns = ns.AddMembers(cls);
                    unit = unit.AddMembers(ns);
                    //Console.WriteLine(ns.NormalizeWhitespace());
                }

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
    }

    public class Config
    {
        /// <summary>
        /// The syntax collection
        /// </summary>
        [XmlElement("Syntax")]
        public List<Syntax> Syntaxes { get; set; }
    }

    public static class StringExt
    {
        public static string ToCamelCase(this string str)
        {
            return char.ToLower(str[0]) + str[1..];
        }
    }


    public abstract class SyntaxArgument
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string Type { get; set; }

        [XmlAttribute] public bool DenyChildrenFill { get; set; }

        [XmlAttribute] public bool PassBase { get; set; }
    }

    public sealed class SyntaxArgumentList : SyntaxArgument
    {
    }

    public sealed class SyntaxArgumentSingle : SyntaxArgument
    {
        /// <summary>
        /// The default value of the argument. It will be passed to the constructor  
        /// </summary>
        [XmlAttribute]
        public string Default { get; set; }
    }

    public class Syntax
    {
        public Syntax()
        {
            Arguments = new List<SyntaxArgument>();
            Base = nameof(SyntaxNode);
        }

        /// <summary>
        /// The name of syntax node
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Base of the current syntax by default it is AstNode
        /// </summary>
        [XmlAttribute]
        public string Base { get; set; }

        [XmlArrayItem("List", typeof(SyntaxArgumentList))]
        [XmlArrayItem("Single", typeof(SyntaxArgumentSingle))]
        public List<SyntaxArgument> Arguments { get; set; }

        /// <summary>
        /// Indicate that the syntax is abstract and only can be driven by another syntax
        /// </summary>
        [XmlAttribute]
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Indicate that the syntax has scope
        /// </summary>
        [XmlAttribute]
        public bool IsScoped { get; set; }

        /// <summary>
        /// Indicate that the syntax relates to the endpoint symbol at the CST
        /// </summary>
        [XmlAttribute]
        public bool IsSymbol { get; set; }

        /// <summary>
        /// Namespace relative to root
        /// </summary>
        [XmlAttribute]
        public string NS { get; set; }

        /// <summary>
        /// Indicates that on generating syntax we not throw the Exceptions relates to the members
        /// </summary>
        [XmlAttribute]
        public bool NotThrowExMembersNotFound { get; set; }
    }
}