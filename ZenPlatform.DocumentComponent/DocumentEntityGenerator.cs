using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using ZenPlatform.Configuration;
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

        /*
         * Необходимо соблюдать формат возвращаемых шаблонов для генерации инструкций
         * Например
         * 
         * Для докумета необходимо, чтобы в чужих свойствах передавался идентификатор для того, чтобы можно было загрузить документ
         * Инструкция get должна выглядить следующим образом:
         * 
         * Session.{ComponentName}.{ObjectName}.Load({Params}) => Session.Document.Invoice.Load(_dto.InvoiceKey)
         * 
         * для инструкции set:
         * 
         * {Valriable} = Session.{ComponentName}.{ObjectName}.GetKey({Value}); => _dto.InvoiceKey = Session.Document.Invoice.GetKey(value);
         */

        public override PGeneratedCodeRule GetInForeignPropertySetActionRule()
        {
            return new PGeneratedCodeRule(PGeneratedCodeRuleType.InForeignPropertyGetActionRule, "Session.{ComponentName}.{ObjectName}.Load({Params})");
        }

        public override PGeneratedCodeRule GetInForeignPropertyGetActionRule()
        {
            return new PGeneratedCodeRule(PGeneratedCodeRuleType.InForeignPropertySetActionRule, "{SetVariable} = Session.{ComponentName}.{ObjectName}.GetKey({Params});");
        }

        public override PGeneratedCodeRule GetClassPostfixRule()
        {
            return new PGeneratedCodeRule(PGeneratedCodeRuleType.EntityClassPostfixRule, "Entity");
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
                        var component = objectProprty.OwnerComponent;

                        var propEnittyPreffix = component.GetCodeRule(PGeneratedCodeRuleType.EntityClassPrefixRule).GetExpression();
                        var propEntityPostfix = component.GetCodeRule(PGeneratedCodeRuleType.EntityClassPostfixRule).GetExpression();

                        var getRule = component.GetCodeRule(PGeneratedCodeRuleType.InForeignPropertyGetActionRule);
                        var setRule = component.GetCodeRule(PGeneratedCodeRuleType.InForeignPropertySetActionRule);

                        var getExp = getRule.GetExpression().NamedFormat(new StandartGetExpressionParameters()
                        {
                            ComponentName = component.Name,
                            ObjectName = propType.Name,
                            Params = $"{DtoPrivateFieldName}.{prop.Name}"
                        });
                        var setExp = setRule.GetExpression().NamedFormat(new StandartSetExpressionParameters()
                        {
                            ComponentName = component.Name,
                            SetVariable = $"{DtoPrivateFieldName}.{prop.Name}",
                            ObjectName = propType.Name,
                            Params = "value"
                        });


                        var getAcessorStatement = new SyntaxNode[]
                        {
                            SyntaxFactory.ParseStatement($"return {getExp};")
                        };

                        var setAcessorStatement = new SyntaxNode[]
                        {
                            SyntaxFactory.ParseStatement($"{setExp}"),
                            SyntaxFactory.ParseStatement("OnPropertyChanged();")
                        };

                        var csProperty =
                            generator.PropertyDeclaration(
                                prop.Name,
                                SyntaxFactory.IdentifierName($"{propEnittyPreffix}{propType.Name}{propEntityPostfix}"),
                                Accessibility.Public,
                                DeclarationModifiers.None, getAcessorStatement, setAcessorStatement);
                        members.Add(csProperty);
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

            var preffix = conf.OwnerComponent.GetCodeRule(PGeneratedCodeRuleType.EntityClassPrefixRule).GetExpression();
            var postfix = conf.OwnerComponent.GetCodeRule(PGeneratedCodeRuleType.EntityClassPostfixRule).GetExpression();

            var classDefinition = generator.ClassDeclaration(
                    $"{preffix}{conf.Name}{postfix}",
                   typeParameters: null,
                   accessibility: Accessibility.Public,
                   modifiers: DeclarationModifiers.Partial,
                   baseType: SyntaxFactory.ParseTypeName("DocumentEntityBase"),
                   interfaceTypes: null,
                   members: members);

            NamespaceDeclarationSyntax namespaceDeclaration =
                generator.NamespaceDeclaration("MyTypes", classDefinition) as NamespaceDeclarationSyntax;

            var newNode = generator.CompilationUnit(classDefinition).NormalizeWhitespace();
            return newNode;
        }


        /*
         * Необходимо сгенерировать Extension methods для класса Session, чтобы
         * появился следующий функционал
         * 
         * Session.Document().Invoice.Load()
         * Session.Document().Invoice.Save()
         * Session.Document().Invoice.Delete()
         * Session.Document().Invoice.Create()
         * 
         * DocumentInterface - класс в котором содержится всё подмножество объектов компонента
         * 
         * public class DocumentInterface
         * {
         *      public DocumentInterface(Session session)
         *      {
         *          Session = session;
         *      }
         *      
         *      public Session Session { get; }
         *      
         *      public DocumentEntityManager<InvoiceEntity> Invoice { get { return new DocumentEntityManager<InvoiceEntity>(Session);} }
         *      
         *      public DocumentEntityManager<InvoiceOutEntity> InvoiceOut { get { return new DocumentEntityManager<InvoiceOutEntity>(Session);} }      
         *      
         *      public DocumentEntityManager<ChequeEntity> Cheque { get { return new DocumentEntityManager<ChequeEntity>(Session);} }      
         *      
         * }
         * 
         * public static class DocumentComponentSessionExtension
         * {
         *      public static DocumentInterface Document(this Session session)
         *      {
         *          return new DocumentInterface(session);
         *      }
         * }
         */


        /// <summary>
        /// Метод для генерации интерфейса сущностей, привязанных к конмоменту
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public SyntaxNode GenerateInterface(PComponent component)
        {
            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                workspace, LanguageNames.CSharp);

            var constructorBody = SyntaxFactory.Block(SyntaxFactory.ParseStatement("Session = session;"));
            var parameters = SyntaxFactory.ParseParameterList("Session session");

            var constructor = generator.ConstructorDeclaration("DocumentInterface",
                parameters.Parameters,
                statements: constructorBody.Statements,
                accessibility: Accessibility.Public);

            var interfaceClass = SyntaxFactory.ClassDeclaration("DocumentInterface")
                .AddMembers(constructor as MemberDeclarationSyntax)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var preffix = component.GetCodeRule(PGeneratedCodeRuleType.EntityClassPrefixRule);
            var postfix = component.GetCodeRule(PGeneratedCodeRuleType.EntityClassPostfixRule);

            foreach (var componentObject in component.Objects)
            {
                var getBody = SyntaxFactory.Block(
                    SyntaxFactory.ParseStatement($"return new DocumentEntityManager<{preffix.GetExpression()}{componentObject.Name}{postfix.GetExpression()}>(Session);")
                    );
                var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, getBody);

                var componentProperty = SyntaxFactory.PropertyDeclaration(
                    SyntaxFactory.ParseTypeName($"DocumentEntityManager<{preffix.GetExpression()}{componentObject.Name}{postfix.GetExpression()}>"),
                    componentObject.Name).
                    WithAccessorList(SyntaxFactory.AccessorList().AddAccessors(getAccessor));

                interfaceClass = interfaceClass.AddMembers(componentProperty);

            }

            var newNode = generator.CompilationUnit(interfaceClass).NormalizeWhitespace();

            return newNode;
        }

        /// <summary>
        /// Метод для генерации компонен
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public SyntaxNode GenerateExtension(PComponent component)
        {
            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                workspace, LanguageNames.CSharp);

            var body = SyntaxFactory.Block(SyntaxFactory.ParseStatement("return new DocumentInterface(session);"));
            var parameters = SyntaxFactory.ParseParameterList("this Session session");

            var method = generator.MethodDeclaration("Document",
                parameters.Parameters,
                statements: body.Statements,
                returnType: SyntaxFactory.ParseTypeName("DocumentInterface"),
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Static);

            var staticClass = SyntaxFactory.ClassDeclaration("DocumentComponentSessionExtension")
                .AddMembers(method as MemberDeclarationSyntax)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));

            var newNode = generator.CompilationUnit(staticClass).NormalizeWhitespace();

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