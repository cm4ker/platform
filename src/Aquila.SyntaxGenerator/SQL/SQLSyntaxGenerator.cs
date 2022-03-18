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

namespace Aquila.SyntaxGenerator.SQL
{
    public static class SQLSyntaxGenerator
    {
        public static NamespaceDeclarationSyntax MakeBaseNode(string rootNameSpace)
        {
            var ns = SyntaxFactory.NamespaceDeclaration(
                SyntaxFactory.ParseName(rootNameSpace));

            var name = "SSyntaxNode";

            List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

            var publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);

            var constructor = SyntaxFactory.ConstructorDeclaration(name)
                .WithBody(SyntaxFactory.Block())
                .WithModifiers(SyntaxTokenList.Create(publicToken));


            var accept =
                (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                    $"public abstract T Accept<T>(QueryVisitorBase<T> visitor);");
            members.Add(accept);

            /*
            var compare =
                (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                    "public bool Compare(QuerySyntaxNode node1, QuerySyntaxNode node2){}");


            compare = compare.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"if (node1==null && node2==null) return true; if (node1==null && node2!=null) return false; if (node1!=null && node2==null) return false; return node1.Equals(node2);"));

            members.Add(compare);
            */


            var cls = SyntaxFactory.ClassDeclaration($"{name}")
                .WithModifiers(SyntaxTokenList.Create(publicToken))
                //  .WithBaseList(SyntaxFactory.BaseList().AddTypes(SyntaxFactory
                //     .SimpleBaseType(SyntaxFactory.ParseTypeName("Aquila.QueryBuilder.Common.SqlNode"))))
                .AddMembers(members.ToArray());


            cls = cls.AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));

            cls = cls.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            return ns.AddMembers(cls);
        }

        public static NamespaceDeclarationSyntax MakeVisitor(string rootNameSpace, List<SQLSyntax> syntaxes)
        {
            var ns = SyntaxFactory.NamespaceDeclaration(
                SyntaxFactory.ParseName(rootNameSpace));

            var name = "QueryVisitorBase";

            List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

            var publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);

            var constructor = SyntaxFactory.ConstructorDeclaration(name)
                .WithBody(SyntaxFactory.Block())
                .WithModifiers(SyntaxTokenList.Create(publicToken));

            foreach (var syntax in syntaxes)
            {
                var visitor =
                    (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                        $"public virtual T Visit{syntax.Name}({syntax.Name} node){{}}");


                visitor = visitor.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"return DefaultVisit(node);"));
                members.Add(visitor);
            }

            var defaultVisitor =
                (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                    $"public virtual T DefaultVisit(SSyntaxNode node){{}}");


            defaultVisitor = defaultVisitor.AddBodyStatements(
                SyntaxFactory.ParseStatement($"return default;"));
            members.Add(defaultVisitor);

            var visit =
                (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                    $"public virtual T Visit(SSyntaxNode visitable){{}}");


            visit = visit.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"if (visitable is null) return default;"))
                .AddBodyStatements(
                    SyntaxFactory.ParseStatement($"return visitable.Accept(this);"));
            members.Add(visit);

            var cls = SyntaxFactory.ClassDeclaration($"{name}<T>")
                .WithModifiers(SyntaxTokenList.Create(publicToken))
                .AddMembers(constructor)
                .AddMembers(members.ToArray());


            cls = cls.AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));

            cls = cls.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            return ns.AddMembers(cls);
        }

        //public static Dictionary<string, List<SyntaxArgument>> constructors = new Dictionary<string, List<SyntaxArgument>>();

        public static NamespaceDeclarationSyntax MakeSyntax(SQLSyntax sqlSyntax, string rootNameSpace, out bool add)
        {
            add = false;
            //Если в аргументах есть свойства которые нужно создавать через new, 
            //то мы смотрим есть ли уже для этого типа аргумента аргументы конструктора, если нету то пропускаем пока этот класс
            // но если тип свойства - список, то пофиг создадим пустой список
            // if (!syntax.Arguments.Where(a => a.IsNeedInitialize() && a.GetType().Equals(typeof(SyntaxArgumentSingle))).SequenceEqual(
            // syntax.Arguments.Where(a => a.IsNeedInitialize() && a.GetType().Equals(typeof(SyntaxArgumentSingle)) && constructors.ContainsKey(a.Type)))) return null;

            var ns = SyntaxFactory.NamespaceDeclaration(
                SyntaxFactory.ParseName(rootNameSpace + (sqlSyntax.NS != null ? "." : "") + sqlSyntax.NS));

            List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();


            var publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);

            var constructor = SyntaxFactory.ConstructorDeclaration(sqlSyntax.Name)
                // .WithParameterList(SyntaxFactory.ParameterList()
                //     .AddParameters(SyntaxFactory.Parameter(
                //         SyntaxFactory.Identifier("lineInfo")).WithType(SyntaxFactory.ParseName("ILineInfo"))))
                .WithBody(SyntaxFactory.Block())
                .WithModifiers(SyntaxTokenList.Create(publicToken));

            var initializer = SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer);

            //var argForConstructor = new List<SyntaxArgument>();

            var cond = new List<string>();
            var hash = new List<string>();

            foreach (var argument in sqlSyntax.Arguments)
            {
                if (argument.Base)
                {
                    initializer = initializer.AddArgumentListArguments(
                        SyntaxFactory.Argument(SyntaxFactory.ParseName(argument.Name.ToCamelCase())));
                }

                if (argument.IsNeedCreate() || (sqlSyntax.Ddl && argument is SyntaxArgumentList))
                {
                    constructor = constructor.AddBodyStatements(
                        SyntaxFactory.ParseStatement($"{argument.Name} = new {argument.Type}();"));
                }


                if (argument.IsNeedInitialize() && !sqlSyntax.Ddl)
                {
                    constructor = constructor.AddBodyStatements(
                        SyntaxFactory.ParseStatement($"{argument.Name} = {argument.Name.ToCamelCase()};"));
                }

                if ((!(argument.Null || argument.IsNeedCreate()) || argument.Base) && !sqlSyntax.Ddl)
                {
                    var parameterSyntax = SyntaxFactory
                        .Parameter(SyntaxFactory.Identifier(argument.Name.ToCamelCase()))
                        .WithType(SyntaxFactory.ParseName(argument.Type));

                    if (argument is SyntaxArgumentSingle sas && sas.Default != null)
                        parameterSyntax = parameterSyntax.WithDefault(
                            SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(sas.Default)));

                    constructor = constructor.AddParameterListParameters(parameterSyntax);
                }


                /*
                else if (argument.IsPrimetive() && !argument.Null)
                {
                    //argForConstructor.Add(argument);
                    var parameterSyntax = SyntaxFactory
                        .Parameter(SyntaxFactory.Identifier(argument.Name.ToCamelCase()))
                        .WithType(SyntaxFactory.ParseName(argument.Type));

                    if (argument is SyntaxArgumentSingle sas && sas.Default != null)
                        parameterSyntax = parameterSyntax.WithDefault(
                            SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(sas.Default)));


                    constructor = constructor.AddParameterListParameters(parameterSyntax);

                    var ae = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.ParseName(argument.Name),
                    SyntaxFactory.ParseName(argument.Name.ToCamelCase()));

                    constructor = constructor.AddBodyStatements(SyntaxFactory.ExpressionStatement(ae));


                }
                */
                if (!argument.Base)
                    members.Add(SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(argument.Type),
                            argument.Name).AddModifiers(publicToken)
                        .WithAccessorList(SyntaxFactory.AccessorList()
                            .AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                            .AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));

                if (argument is SyntaxArgumentList)
                {
                    hash.Add($"Xor({argument.Name},i => i.GetHashCode())");
                    cond.Add($"SequenceEqual(this.{argument.Name},node.{argument.Name})");
                }
                else if (argument.IsPrimetive())
                {
                    hash.Add($"({argument.Name}.GetHashCode())");
                    cond.Add($"(this.{argument.Name} == node.{argument.Name})");
                }
                else if (argument.Type == "object")
                {
                    hash.Add($"({argument.Name} == null ? 0: {argument.Name}.GetHashCode())");
                    cond.Add($"(this.{argument.Name}.Equals(node.{argument.Name}))");
                }
                else
                {
                    hash.Add($"({argument.Name} == null ? 0: {argument.Name}.GetHashCode())");
                    cond.Add($"Compare(this.{argument.Name}, node.{argument.Name})");
                }
            }

            constructor = constructor.WithInitializer(initializer);
            if (cond.Count > 0)
            {
                var equlse =
                    (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                        $"public override bool Equals(object obj){{}}");
                equlse = equlse.AddBodyStatements(
                    SyntaxFactory.ParseStatement(
                        $"if (!this.GetType().Equals(obj.GetType())) return false;\nvar node = ({sqlSyntax.Name})obj;\nreturn ({string.Join(" && ", cond)});"));
                members.Add(equlse);

                var getHash =
                    (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                        "public override int GetHashCode(){}");


                getHash = getHash.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"return {string.Join(" ^ ", hash)};"));


                members.Add(getHash);
            }
            else
            {
                var getHash =
                    (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                        "public override int GetHashCode(){}");


                getHash = getHash.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"return base.GetHashCode();"));


                members.Add(getHash);
            }

            var visitor =
                (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                    "public override T Accept<T>(QueryVisitorBase<T> visitor){}");


            visitor = visitor.AddBodyStatements(
                SyntaxFactory.ParseStatement($"return visitor.Visit{sqlSyntax.Name}(this);"));
            members.Add(visitor);


            var cls = SyntaxFactory.ClassDeclaration(sqlSyntax.Name)
                .WithModifiers(SyntaxTokenList.Create(publicToken))
                .WithBaseList(SyntaxFactory.BaseList().AddTypes(SyntaxFactory
                    .SimpleBaseType(SyntaxFactory.ParseTypeName(sqlSyntax.Base))))
                .AddMembers(constructor)
                .AddMembers(members.ToArray());


            cls = cls.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));


            add = true;

            return ns.AddMembers(cls);
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine($"Invalid using. Use {nameof(Aquila.SyntaxGenerator)} [PathToXmlScheme]");

            using (TextReader tr = new StreamReader(args[0]))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                var root = (Config) xs.Deserialize(tr);

                var unit = SyntaxFactory.CompilationUnit()
                    .AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Linq")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Aquila.QueryBuilder.Model")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Aquila.QueryBuilder.Visitor"))
                    );
                ;


                unit = unit.AddMembers(MakeBaseNode("Aquila.QueryBuilder.Model"));


                List<SQLSyntax> compliteSyntaxList = new List<SQLSyntax>();
                while (root.Syntaxes.Count != compliteSyntaxList.Count)
                    foreach (var syntax in root.Syntaxes.Where(s => !compliteSyntaxList.Contains(s)))
                    {
                        bool add = false;
                        var ns = MakeSyntax(syntax, "Aquila.QueryBuilder.Model", out add);
                        if (add)
                        {
                            compliteSyntaxList.Add(syntax);
                            unit = unit.AddMembers(ns);
                        }
                    }

                unit = unit.AddMembers(MakeVisitor("Aquila.QueryBuilder.Visitor", root.Syntaxes));

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
}