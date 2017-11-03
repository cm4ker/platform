using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using ZenPlatform.Configuration.Data;
using ZenPlatform.ConfigurationDataComponent;
using ZenPlatform.Core;
using ZenPlatform.Core.Entity;
using ZenPlatform.CSharpCodeBuilder.Syntax;
using ZenPlatform.DataComponent;
using Environment = ZenPlatform.Core.Environment;

namespace ZenPlatform.DocumentComponent
{
    public class DocumentEntityGenerator : EntityGeneratorBase
    {
        public DocumentEntityGenerator()
        {
        }

        protected AccessorListSyntax GetStandardAccessorListSyntax()
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
        protected PropertyDeclarationSyntax GetStandartProperty(string name, string type)
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
                    {
                        members.Add(GetStandartProperty($"{prop.Name}_Ref", "Guid"));
                    }

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

            var newNode = generator.CompilationUnit(classDefinition).NormalizeWhitespace();
            return newNode;
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
                          SyntaxFactory.ParseStatement($"return _dto.{prop.Name};")
                        };

                        var setAcessorStatement = new SyntaxNode[]
                          {
                              SyntaxFactory.ParseStatement($"_dto.{prop.Name} = value;"),
                              SyntaxFactory.ParseStatement("OnPropertyChanged();")
                    };

                        var csProperty =
                            generator.PropertyDeclaration(
                                prop.Name,
                                SyntaxFactory.IdentifierName(primitiveType.CLRType.CSharpName()),
                                Accessibility.Public,
                                DeclarationModifiers.None, getAcessorStatement, setAcessorStatement);
                        members.Add(csProperty);
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

            var statementsBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement("PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));")
            };

            var parameterList = SyntaxFactory.ParseParameterList("[CallerMemberName] string propertyName = null");
            var statementsParams = new SyntaxNode[]
            {
                parameterList.Parameters.First()
            };

            var npcMethod = generator.MethodDeclaration("OnPropertyChanged", statements: statementsBody, parameters: statementsParams);
            var npcEvent = generator.EventDeclaration("PropertyChanged",
                SyntaxFactory.IdentifierName("PropertyChangedEventHandler"));

            members.Add(npcEvent);
            members.Add(npcMethod);

            parameterList = SyntaxFactory.ParseParameterList($"[NotNull] {nameof(Session)} session, {conf.Name}Dto dto");
            statementsParams = parameterList.Parameters.ToArray<SyntaxNode>();


            statementsBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement("Session = session;"),
                SyntaxFactory.ParseStatement("Environment = session.Environment;"),
                SyntaxFactory.ParseStatement("_dto = dto;")
            };

            var constructor = generator.ConstructorDeclaration(null, statementsParams, Accessibility.Public, statements: statementsBody);

            var sessionGetBody = SyntaxFactory.Block
             (
                SyntaxFactory.ParseStatement($"return session;")
            );

            var environmentGetBody = SyntaxFactory.Block(

                SyntaxFactory.ParseStatement($"return session.Environment;")
            );

            var sessionProperty = SyntaxFactory.PropertyDeclaration(
                    SyntaxFactory.ParseTypeName(nameof(Session)),
                    "Session"
                )
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithBody(sessionGetBody)
                );

            //var sessionProperty = generator.PropertyDeclaration("Session",
            //    SyntaxFactory.IdentifierName(nameof(Session)), Accessibility.Public,
            //    getAccessorStatements: sessionGetAcessorStatement);

            var environmentProperty = SyntaxFactory.PropertyDeclaration(
                    SyntaxFactory.ParseTypeName(nameof(Environment)),
                    "Environment"
                )
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithBody(environmentGetBody)
                );

            //var environmentProperty = generator.PropertyDeclaration("Environment",
            //    SyntaxFactory.IdentifierName(nameof(Environment)), Accessibility.Public,
            //    getAccessorStatements: environmentGetBody);

            members.Insert(0, environmentProperty);
            members.Insert(0, sessionProperty);
            members.Insert(0, constructor);
            members.Insert(0, dtoPrivateField);

            var classDefinition = generator.ClassDeclaration(
                    $"{conf.Name}Entity",
                   typeParameters: null,
                   accessibility: Accessibility.Public,
                   modifiers: DeclarationModifiers.None,
                   baseType: null,
                   interfaceTypes: null,
                   members: members);

            NamespaceDeclarationSyntax namespaceDeclaration =
                generator.NamespaceDeclaration("MyTypes", classDefinition) as NamespaceDeclarationSyntax;

            var newNode = generator.CompilationUnit(classDefinition).NormalizeWhitespace();
            return newNode;
        }

    }
}