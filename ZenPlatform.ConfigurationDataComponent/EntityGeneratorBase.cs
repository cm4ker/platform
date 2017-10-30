using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using ZenPlatform.Configuration.Data;
using ZenPlatform.CSharpCodeBuilder.Syntax;

namespace ZenPlatform.DataComponent
{
    public abstract class EntityGeneratorBase
    {
        protected EntityGeneratorBase()
        {
        }


        public AccessorListSyntax GetStandardAccessorListSyntax()
        {
            return SyntaxFactory.AccessorList(
                SyntaxFactory.List(
                    new[]
                    {
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    }));
        }

        public PropertyDeclarationSyntax GetStandartProperty(string name, string type)
        {
            return
                SyntaxFactory.PropertyDeclaration(
                    SyntaxFactory.ParseTypeName(type),
                    name).WithAccessorList(GetStandardAccessorListSyntax());
        }

        public virtual SyntaxNode GenerateDtoClass(PObjectType conf)
        {
            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                  workspace, LanguageNames.CSharp);

            var usingDirectives = generator.NamespaceImportDeclaration("System");

            if (conf.IsAbstractType) return null;

            var members = new List<SyntaxNode>();

            members.Add(GetStandartProperty("Key", "Guid"));

            foreach (var prop in conf.Propertyes)
            {
                if (prop.Types.Count == 1)
                {
                    var propType = prop.Types.First();

                    if (propType is PObjectType)
                        members.Add(GetStandartProperty(prop.Name, propType.Name));

                    if (propType is PPrimetiveType primitiveType)
                        members.Add(GetStandartProperty(prop.Name, primitiveType.CLRType.CSharpName()));
                }
                else
                {
                    bool alreadyHaveObjectTypeField = false;
                    foreach (var type in prop.Types)
                    {
                        if (type is PObjectType && !alreadyHaveObjectTypeField)
                        {
                            members.Add(GetStandartProperty($"{prop.Name}_Ref", "Guid"));
                            members.Add(GetStandartProperty($"{prop.Name}_Type", "int"));
                            alreadyHaveObjectTypeField = true;
                        }

                        if (type is PPrimetiveType primitiveType)
                            members.Add(GetStandartProperty($"{prop.Name}_{primitiveType.Name}",
                                primitiveType.CLRType.CSharpName()));
                    }
                }
            }

            var classDefinition = generator.ClassDeclaration(
                   $"{conf.Name}Dto",
                   typeParameters: null,
                   accessibility: Accessibility.Public,
                   modifiers: DeclarationModifiers.None,
                   baseType: null,
                   interfaceTypes: null,
                   members: members);

            NamespaceDeclarationSyntax namespaceDeclaration =
                generator.NamespaceDeclaration("EntityDeclaration", classDefinition) as NamespaceDeclarationSyntax;

            var newNode = generator.CompilationUnit(usingDirectives, namespaceDeclaration).NormalizeWhitespace();
            return newNode;
        }

        private string StartLowerCase(string str)
        {
            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public virtual SyntaxNode GenerateEntityClass(PObjectType conf)
        {
            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                  workspace, LanguageNames.CSharp);

            var usingDirectives = generator.NamespaceImportDeclaration("System");

            if (conf.IsAbstractType) return null;

            var members = new List<SyntaxNode>();

            var dtoPrivateField = generator.FieldDeclaration($"_dto", SyntaxFactory.ParseTypeName($"{conf.Name}Dto"));
            members.Add(dtoPrivateField);

            foreach (var prop in conf.Propertyes)
            {
                if (prop.Types.Count == 1)
                {
                    var propType = prop.Types.First();

                    if (propType is PObjectType)
                    {
                        members.Add(GetStandartProperty(prop.Name, propType.Name));
                    }

                    if (propType is PPrimetiveType primitiveType)
                    {
                        var getAcessorStatement = new SyntaxNode[]
                        {
                            generator.ReturnStatement(generator.IdentifierName("_lastName"))
                        };

                        members.Add(GetStandartProperty(prop.Name, primitiveType.CLRType.CSharpName()));
                    }
                }
                else
                {
                    bool alreadyHaveObjectTypeField = false;
                    foreach (var type in prop.Types)
                    {
                        if (type is PObjectType && !alreadyHaveObjectTypeField)
                        {
                            members.Add(GetStandartProperty($"{prop.Name}_Ref", "Guid"));
                            members.Add(GetStandartProperty($"{prop.Name}_Type", "int"));
                            alreadyHaveObjectTypeField = true;
                        }

                        if (type is PPrimetiveType primitiveType)
                            members.Add(GetStandartProperty($"{prop.Name}_{primitiveType.Name}",
                                primitiveType.CLRType.CSharpName()));
                    }
                }
            }

            var classDefinition = generator.ClassDeclaration(
                    $"{conf.Name}Eentity",
                   typeParameters: null,
                   accessibility: Accessibility.Public,
                   modifiers: DeclarationModifiers.None,
                   baseType: null,
                   interfaceTypes: null,
                   members: members);

            NamespaceDeclarationSyntax namespaceDeclaration =
                generator.NamespaceDeclaration("MyTypes", classDefinition) as NamespaceDeclarationSyntax;

            var newNode = generator.CompilationUnit(usingDirectives, namespaceDeclaration).NormalizeWhitespace();
            return newNode;
        }
    }
}