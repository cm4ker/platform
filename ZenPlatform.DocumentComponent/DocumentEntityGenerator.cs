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
                    name).WithAccessorList(GetStandardAccessorListSyntax())
                    .WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));
        }
        public virtual SyntaxNode GenerateDtoClass(PObjectType conf)
        {
            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                  workspace, LanguageNames.CSharp);

            var usingDirectives = generator.NamespaceImportDeclaration("System");

            if (conf.IsAbstractType) return null;

            var members = new List<SyntaxNode>();

            foreach (var prop in conf.Properties)
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
                   $"{conf.Name}{DtoPrefix}",
                   typeParameters: null,
                   accessibility: Accessibility.Public,
                   modifiers: DeclarationModifiers.None,
                   baseType: null,
                   interfaceTypes: null,
                   members: members);

            var newNode = generator.CompilationUnit(classDefinition).NormalizeWhitespace();
            return newNode;
        }

        /// <summary>
        /// Генерация сущности - класс который является промежуточным между DTO и пользователем
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        public virtual SyntaxNode GenerateEntityClass(PObjectType conf)
        {
            var dtoClassName = $"{conf.Name}{DtoPrefix}";

            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                  workspace, LanguageNames.CSharp);

            if (conf.IsAbstractType) return null;

            var members = new List<SyntaxNode>();

            var dtoPrivateField = generator.FieldDeclaration(DtoPrivateFieldName, SyntaxFactory.ParseTypeName(dtoClassName));


            //TODO: Алгоритм для обхода полей объекта
            /*
                Если поле PrimitiveType, при условии что этот тип один - взять имя свойства как имя поля, взять тип свойства как тип поля
                Если поле ObjectType(Другой объект конфигурации) - Взять и имя свойства, как имя свойста, взять тип, как наименование типа Entity
                                
            */
            foreach (var prop in conf.Properties)
            {
                if (prop.Types.Count == 1)
                {
                    var propType = prop.Types.First();

                    if (propType is PObjectType objectProprty)
                    {
                        //var component = objectProprty.OwnerComponent.GetCodeRule();
                        //var pobjectProperty = SyntaxFactory.PropertyDeclaration();

                        members.Add(null);
                        //members.Add(GetStandartProperty(prop.Name, propType.Name));
                    }

                    if (propType is PPrimetiveType primitiveType)
                    {
                        var getAcessorStatement = new SyntaxNode[]
                        {
                          SyntaxFactory.ParseStatement($"return {DtoPrivateFieldName}.{prop.Name};")
                        };

                        var setAcessorStatement = new SyntaxNode[]
                          {
                              SyntaxFactory.ParseStatement($"{DtoPrivateFieldName}.{prop.Name} = value;"),
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


            var parameterList = SyntaxFactory.ParseParameterList($"[NotNull] {nameof(Session)} session, {dtoClassName} dto");
            var statementsParams = parameterList.Parameters.ToArray<SyntaxNode>();
            var baseConstructorArguments = SyntaxFactory.ArgumentList();
            baseConstructorArguments =
                baseConstructorArguments.AddArguments(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("session")));

            var statementsBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement($"{DtoPrivateFieldName} = dto;")
            };

            var constructor = generator.ConstructorDeclaration(null, statementsParams, Accessibility.Public, statements: statementsBody, baseConstructorArguments: baseConstructorArguments.Arguments);


            members.Insert(0, constructor);
            members.Insert(0, dtoPrivateField);

            #region CRUD methods

            var saveBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement("throw new NotImplementedException();")
            };
            var saveMethod = generator.MethodDeclaration("Save", statements: saveBody, accessibility: Accessibility.Public);

            var loadBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement("throw new NotImplementedException();")
            };
            var loadMethod = generator.MethodDeclaration("Load", statements: loadBody, accessibility: Accessibility.Public);

            var deleteBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement("throw new NotImplementedException();")
            };
            var deleteMethod = generator.MethodDeclaration("Delete", statements: deleteBody, accessibility: Accessibility.Public);

            members.Add(saveMethod);
            members.Add(loadMethod);
            members.Add(deleteMethod);
            #endregion

            var classDefinition = generator.ClassDeclaration(
                    $"{conf.Name}Entity",
                   typeParameters: null,
                   accessibility: Accessibility.Public,
                   modifiers: DeclarationModifiers.None,
                   baseType: SyntaxFactory.ParseTypeName("DocumentEntityBase"),
                   interfaceTypes: null,
                   members: members);

            NamespaceDeclarationSyntax namespaceDeclaration =
                generator.NamespaceDeclaration("MyTypes", classDefinition) as NamespaceDeclarationSyntax;

            var newNode = generator.CompilationUnit(classDefinition).NormalizeWhitespace();
            return newNode;
        }

        /*
        TODO: Необходимо сгенерировать следующее API для работы с сущностями:
              Session.Document.Invoice.Save();
              Session.Register.Incommings.GetRange();
              Session.Reference.Nomenclature.Create();
         */

        public SyntaxNode GenerateHelpersForEntity()
        {
            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                workspace, LanguageNames.CSharp);

            var managerClass = SyntaxFactory.ClassDeclaration("ManagerExtension")
                                    .WithModifiers(
                                    SyntaxTokenList.Create(
                                        SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                        .Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

            var field = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("SomeType"), "SomeName")
                .WithModifiers(
                SyntaxTokenList.Create(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

            managerClass = managerClass.AddMembers(field);

            return generator.CompilationUnit(managerClass).NormalizeWhitespace(); ;

        }

        /*
        public static partial class DocumentsManager
        {
            public static DocumentManagerBase<Invoice> InvoiceManager(this Session session)
            {
                return session.Environment.Managers[0] as DocumentManagerBase<Invoice>;
            }
        }
        */
    }
}