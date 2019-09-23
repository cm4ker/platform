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

namespace ZenPlatform.SyntaxGenerator.SQL
{
    public static class SQLSyntaxGenerator
    {

        public static NamespaceDeclarationSyntax MakeBaseNode(string rootNameSpace)
        {
            var ns = SyntaxFactory.NamespaceDeclaration(
                        SyntaxFactory.ParseName(rootNameSpace));

            var name = "QuerySyntaxNode";

            List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

            var publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);

            var constructor = SyntaxFactory.ConstructorDeclaration(name)
                .WithBody(SyntaxFactory.Block())
                .WithModifiers(SyntaxTokenList.Create(publicToken));


           var accept =
                    (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                        $"public abstract T Accept<T>(QueryVisitorBase<T> visitor);");


           members.Add(accept);



            var cls = SyntaxFactory.ClassDeclaration($"{name}")
                .WithModifiers(SyntaxTokenList.Create(publicToken))
               // .AddMembers(constructor)
                .AddMembers(members.ToArray());


            cls = cls.AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));


            return ns.AddMembers(cls);
        }

        public static NamespaceDeclarationSyntax MakeVisitor(string rootNameSpace, List<Syntax> syntaxes)
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
                    (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                        $"public virtual T Visit{syntax.Name}({syntax.Name} node){{}}");


                visitor = visitor.AddBodyStatements(
                        SyntaxFactory.ParseStatement($"return DefaultVisit(node);"));
                members.Add(visitor);
            }

            var defaultVisitor =
                    (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                        $"public virtual T DefaultVisit(QuerySyntaxNode node){{}}");


            defaultVisitor = defaultVisitor.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"return default;"));
            members.Add(defaultVisitor);

            var visit =
                    (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                        $"public virtual T Visit(QuerySyntaxNode visitable){{}}");


            visit = visit.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"if (visitable is null) return default;"))
                    .AddBodyStatements(
                    SyntaxFactory.ParseStatement($"return visitable.Accept(this);"));
            members.Add(visit);

            var cls = SyntaxFactory.ClassDeclaration($"{name}<T>")
                .WithModifiers(SyntaxTokenList.Create(publicToken))
                .AddMembers(constructor)
                .AddMembers(members.ToArray());
            

            cls = cls.AddModifiers( SyntaxFactory.Token(SyntaxKind.AbstractKeyword));


            return ns.AddMembers(cls);
        }

        //public static Dictionary<string, List<SyntaxArgument>> constructors = new Dictionary<string, List<SyntaxArgument>>();

        public static NamespaceDeclarationSyntax MakeSyntax(Syntax syntax, string rootNameSpace, out bool add)
        {
            add = false;
            //Если в аргументах есть свойства которые нужно создавать через new, 
            //то мы смотрим есть ли уже для этого типа аргумента аргументы конструктора, если нету то пропускаем пока этот класс
            // но если тип свойства - список, то пофиг создадим пустой список
           // if (!syntax.Arguments.Where(a => a.IsNeedInitialize() && a.GetType().Equals(typeof(SyntaxArgumentSingle))).SequenceEqual(
           // syntax.Arguments.Where(a => a.IsNeedInitialize() && a.GetType().Equals(typeof(SyntaxArgumentSingle)) && constructors.ContainsKey(a.Type)))) return null;

            var ns = SyntaxFactory.NamespaceDeclaration(
                        SyntaxFactory.ParseName(rootNameSpace + (syntax.NS != null ? "." : "") + syntax.NS));

            List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();


            var publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);

            var constructor = SyntaxFactory.ConstructorDeclaration(syntax.Name)
                // .WithParameterList(SyntaxFactory.ParameterList()
                //     .AddParameters(SyntaxFactory.Parameter(
                //         SyntaxFactory.Identifier("lineInfo")).WithType(SyntaxFactory.ParseName("ILineInfo"))))
                .WithBody(SyntaxFactory.Block())
                .WithModifiers(SyntaxTokenList.Create(publicToken));

            //var initializer = SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
            //    SyntaxFactory.ArgumentList()
            //       .AddArguments(SyntaxFactory.Argument(SyntaxFactory.ParseName("lineInfo"))));

            //var argForConstructor = new List<SyntaxArgument>();


            foreach (var argument in syntax.Arguments)
            {
                if (argument.IsNeedInitialize())
                {

                        constructor = constructor.AddBodyStatements(
                        SyntaxFactory.ParseStatement($"{argument.Name} = new {argument.Type}();"));
                    
                }
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

                members.Add(SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(argument.Type),
                                argument.Name).AddModifiers(publicToken)
                            .WithAccessorList(SyntaxFactory.AccessorList()
                                .AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                                .AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));

                /*
                members.Add(SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(argument.Type),
                        argument.Name.ToUpCase()).AddModifiers(publicToken)
                    .WithAccessorList(SyntaxFactory.AccessorList()
                        .AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .AddBodyStatements(SyntaxFactory.ParseStatement($"return _{argument.Name.ToCamelCase()};")))
                        .AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .AddBodyStatements(SyntaxFactory.ParseStatement($"ReplaceOrAttach(_{argument.Name.ToCamelCase()}, value);"))
                            .AddBodyStatements(SyntaxFactory.ParseStatement($"\r\n_{argument.Name.ToCamelCase()} = value;")))));

                members.Add(SyntaxFactory.FieldDeclaration(SyntaxFactory.
                        VariableDeclaration(SyntaxFactory.ParseTypeName(argument.Type)).
                        AddVariables(SyntaxFactory.VariableDeclarator($"_{argument.Name.ToCamelCase()}"))
                        ).WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))));
                */
                // constructor = constructor.WithInitializer(initializer);
            }
            //constructors.Add(syntax.Name, argForConstructor);
            var visitor =
                (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
                    "public override T Accept<T>(QueryVisitorBase<T> visitor){}");


            visitor = visitor.AddBodyStatements(
                    SyntaxFactory.ParseStatement($"return visitor.Visit{syntax.Name}(this);"));

            members.Add(visitor);

            var cls = SyntaxFactory.ClassDeclaration(syntax.Name)
                .WithModifiers(SyntaxTokenList.Create(publicToken))
                .WithBaseList(SyntaxFactory.BaseList().AddTypes(SyntaxFactory
                    .SimpleBaseType(SyntaxFactory.ParseTypeName(syntax.Base))))
                .AddMembers(constructor)
                .AddMembers(members.ToArray());


            cls = cls.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));



            add = true;

            return ns.AddMembers(cls);
            
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine($"Invalid using. Use {nameof(ZenPlatform.SyntaxGenerator)} [PathToXmlScheme]");

            using (TextReader tr = new StreamReader(args[0]))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                var root = (Config)xs.Deserialize(tr);

                var unit = SyntaxFactory.CompilationUnit()
                    .AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Text")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ZenPlatform.QueryBuilder.Model")),
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ZenPlatform.QueryBuilder.Visitor")),
                         SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ZenPlatform.QueryBuilder.Contracts"))
                    );
                ;


                unit = unit.AddMembers(MakeBaseNode("ZenPlatform.QueryBuilder.Model"));


                List<Syntax> compliteSyntaxList = new List<Syntax>();
                while (root.Syntaxes.Count != compliteSyntaxList.Count)
                foreach (var syntax in root.Syntaxes.Where(s=>!compliteSyntaxList.Contains(s)))
                {
                        bool add = false;
                        var ns = MakeSyntax(syntax, "ZenPlatform.QueryBuilder.Model", out add);
                        if (add)
                        {
                            compliteSyntaxList.Add(syntax);
                            unit = unit.AddMembers(ns);
                        }
                }

                unit = unit.AddMembers(MakeVisitor("ZenPlatform.QueryBuilder.Visitor", root.Syntaxes));

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

        public static string ToUpCase(this string str)
        {
            return char.ToUpper(str[0]) + str[1..];
        }
    }


    public abstract class SyntaxArgument
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public virtual string Type { get; set; }
        [XmlAttribute] public bool Null { get; set; }


        public bool IsPrimetive()
        {
            return new string[] { "int", "string", "float", "bool" }.Contains(Type);
        }
        public bool IsNeedInitialize()
        {

            return this.GetType().Equals(typeof(SyntaxArgumentList)) && !Null;
        }
    }

    public sealed class SyntaxArgumentList : SyntaxArgument
    {
        private string _type;
        [XmlAttribute]
        public override string Type {
            get
            {
                return $"List<{_type}>";
            }
            set
            {
                _type = value;
            }
        }
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
            Base = "QuerySyntaxNode";
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

        [XmlAttribute]
        public string NS { get; set; }
    }
}
