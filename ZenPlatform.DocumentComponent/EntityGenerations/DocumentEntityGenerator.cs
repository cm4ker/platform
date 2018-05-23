using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.Core;
using ZenPlatform.Core.Entity;
using ZenPlatform.CSharpCodeBuilder.Syntax;
using ZenPlatform.DataComponent;
using ZenPlatform.DocumentComponent.Configuration;

namespace ZenPlatform.DocumentComponent
{
    public class DocumentEntityGenerator : EntityGeneratorBase
    {


        public DocumentEntityGenerator(PComponent component) : base(component)
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
            return new PGeneratedCodeRule(PGeneratedCodeRuleType.InForeignPropertyGetActionRule, "Session.{ComponentName}().{ObjectName}.Load({Params})");
        }

        public override PGeneratedCodeRule GetInForeignPropertyGetActionRule()
        {
            return new PGeneratedCodeRule(PGeneratedCodeRuleType.InForeignPropertySetActionRule, "{SetVariable} = Session.{ComponentName}().{ObjectName}.GetKey({Params});");
        }

        public override PGeneratedCodeRule GetEntityClassPostfixRule()
        {
            return new PGeneratedCodeRule(PGeneratedCodeRuleType.EntityClassPostfixRule, "Entity");
        }

        public virtual SyntaxNode GenerateDtoClass(PObjectType conf)
        {
            var component = conf.OwnerComponent;
            var nsRule = component.GetCodeRule(PGeneratedCodeRuleType.NamespaceRule);

            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                  workspace, LanguageNames.CSharp);

            var usings = generator.NamespaceImportDeclaration("System");

            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsRule.GetExpression()));

            if (conf.IsAbstract) return null;

            var members = new List<SyntaxNode>();

            foreach (var prop in conf.Properties)
            {
                if (prop.Types.Count == 1)
                {
                    var propType = prop.Types.First();

                    if (propType is PObjectType)
                    {
                        members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_Ref", "Guid"));
                    }

                    if (propType is PPrimetiveType primitiveType)
                        members.Add(GetPropertyWithEmptyAccessor(prop.DatabaseColumnName, primitiveType.CLRType.CSharpName()));
                }
                else
                {
                    bool alreadyHaveObjectTypeField = false;

                    foreach (var type in prop.Types)
                    {
                        if (type is PObjectType && !alreadyHaveObjectTypeField)
                        {
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_Ref", "Guid"));
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_Type", "int"));

                            alreadyHaveObjectTypeField = true;
                        }

                        if (type is PPrimetiveType primitiveType)
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_{primitiveType.Name}",
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
                   members: members) as MemberDeclarationSyntax;


            ns = ns.AddMembers(classDefinition);

            var newNode = SyntaxFactory.CompilationUnit()
                .AddUsings(GetStandartNamespaces().ToArray())
                .AddMembers(ns).NormalizeWhitespace();
            return newNode;
        }

        /// <summary>
        /// Генерация сущности - класс который является промежуточным между DTO и пользователем
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        public virtual SyntaxNode GenerateEntityClass(PObjectType conf)
        {
            var component = conf.OwnerComponent;



            var dtoClassName = $"{conf.Name}{DtoPrefix}";

            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                  workspace, LanguageNames.CSharp);


            var nsRule = component.GetCodeRule(PGeneratedCodeRuleType.NamespaceRule);
            var usings = generator.NamespaceImportDeclaration("System");
            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsRule.GetExpression()));

            if (conf.IsAbstract) return null;

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
                        var propertyComponent = objectProprty.OwnerComponent;

                        var propEnittyPreffix = propertyComponent.GetCodeRule(PGeneratedCodeRuleType.EntityClassPrefixRule).GetExpression();
                        var propEntityPostfix = propertyComponent.GetCodeRule(PGeneratedCodeRuleType.EntityClassPostfixRule).GetExpression();

                        var getRule = propertyComponent.GetCodeRule(PGeneratedCodeRuleType.InForeignPropertyGetActionRule);
                        var setRule = propertyComponent.GetCodeRule(PGeneratedCodeRuleType.InForeignPropertySetActionRule);

                        var getExp = getRule.GetExpression().NamedFormat(new StandartGetExpressionParameters()
                        {
                            ComponentName = propertyComponent.Name,
                            ObjectName = propType.Name,
                            Params = $"{DtoPrivateFieldName}.{prop.DatabaseColumnName}_Ref"
                        });
                        var setExp = setRule.GetExpression().NamedFormat(new StandartSetExpressionParameters()
                        {
                            ComponentName = propertyComponent.Name,
                            SetVariable = $"{DtoPrivateFieldName}.{prop.DatabaseColumnName}_Ref",
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
                                prop.DatabaseColumnName,
                                SyntaxFactory.IdentifierName($"{propEnittyPreffix}{propType.Name}{propEntityPostfix}"),
                                Accessibility.Public,
                                DeclarationModifiers.None, getAcessorStatement, setAcessorStatement);
                        members.Add(csProperty);
                    }

                    if (propType is PPrimetiveType primitiveType)
                    {
                        var getAcessorStatement = new SyntaxNode[]
                        {
                          SyntaxFactory.ParseStatement($"return {DtoPrivateFieldName}.{prop.DatabaseColumnName};")
                        };

                        var setAcessorStatement = new SyntaxNode[]
                          {
                              SyntaxFactory.ParseStatement($"{DtoPrivateFieldName}.{prop.DatabaseColumnName} = value;"),
                              SyntaxFactory.ParseStatement("OnPropertyChanged();")
                    };

                        var csProperty =
                            generator.PropertyDeclaration(
                                prop.DatabaseColumnName,
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
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_Ref", "Guid"));
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_Type", "int"));
                            alreadyHaveObjectTypeField = true;
                        }

                        if (type is PPrimetiveType primitiveType)
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_{primitiveType.Name}",
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
                SyntaxFactory.ParseStatement($"Session.{component.Name}().{conf.Name}.Save(this);")
            };
            var saveMethod = generator.MethodDeclaration("Save", statements: saveBody, accessibility: Accessibility.Public);

            var loadBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement($"var key = Session.{component.Name}().{conf.Name}.GetKey(this);"),
                SyntaxFactory.ParseStatement($"var entity = Session.{component.Name}().{conf.Name}.Load(key);"),
                SyntaxFactory.ParseStatement($"_dto = entity._dto;"),
            };
            var loadMethod = generator.MethodDeclaration("Load", statements: loadBody, accessibility: Accessibility.Public);

            var deleteBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement($"Session.{component.Name}().{conf.Name}.Delete(this);")
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
                   baseType: SyntaxFactory.ParseTypeName(nameof(DocumentEntity)),
                   interfaceTypes: null,
                   members: members) as MemberDeclarationSyntax;


            ns = ns.AddMembers(classDefinition);

            var newNode = SyntaxFactory.CompilationUnit()
                .AddUsings(GetStandartNamespaces().ToArray())
                .AddMembers(ns).NormalizeWhitespace();

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
        public SyntaxNode GenerateInterface()
        {
            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                workspace, LanguageNames.CSharp);

            var nsRule = Component.GetCodeRule(PGeneratedCodeRuleType.NamespaceRule);
            var systemUsing = generator.NamespaceImportDeclaration("System");
            var coreUsing = generator.NamespaceImportDeclaration(nameof(ZenPlatform.Core));


            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsRule.GetExpression()));


            var constructorBody = SyntaxFactory.Block(SyntaxFactory.ParseStatement("Session = session;"));
            var parameters = SyntaxFactory.ParseParameterList($"{nameof(Session)} session");

            var constructor = generator.ConstructorDeclaration("DocumentInterface",
                parameters.Parameters,
                statements: constructorBody.Statements,
                accessibility: Accessibility.Public);

            var sessionProperty =
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(nameof(Session)), "Session")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                .WithAccessorList(
                    SyntaxFactory.AccessorList().AddAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))));

            var interfaceClass = SyntaxFactory.ClassDeclaration("DocumentInterface")
                .AddMembers(constructor as MemberDeclarationSyntax)
                .AddMembers(sessionProperty)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var preffix = Component.GetCodeRule(PGeneratedCodeRuleType.EntityClassPrefixRule);
            var postfix = Component.GetCodeRule(PGeneratedCodeRuleType.EntityClassPostfixRule);

            foreach (var componentObject in GetTypes<PDocumentObjectType>())
            {
                var getBody = SyntaxFactory.Block(
                    SyntaxFactory.ParseStatement($"return new DocumentEntityManager<{preffix.GetExpression()}{componentObject.Name}{postfix.GetExpression()}>(Session);")
                    );
                var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, getBody);

                var componentProperty = SyntaxFactory.PropertyDeclaration(
                    SyntaxFactory.ParseTypeName($"DocumentEntityManager<{preffix.GetExpression()}{componentObject.Name}{postfix.GetExpression()}>"),
                    componentObject.Name).
                    WithAccessorList(SyntaxFactory.AccessorList().AddAccessors(getAccessor))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                interfaceClass = interfaceClass.AddMembers(componentProperty);
            }

            ns = ns.AddMembers(interfaceClass);

            var newNode = SyntaxFactory.CompilationUnit()
                .AddUsings(GetStandartNamespaces().ToArray())
                .AddMembers(ns).NormalizeWhitespace();

            return newNode;
        }

        /// <summary>
        /// Метод для генерации расширения к классу Session
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public SyntaxNode GenerateExtension()
        {
            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                workspace, LanguageNames.CSharp);

            var nsRule = Component.GetCodeRule(PGeneratedCodeRuleType.NamespaceRule);
            var usings = generator.NamespaceImportDeclaration("System");
            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsRule.GetExpression()));


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

            ns = ns.AddMembers(staticClass);

            var newNode = SyntaxFactory.CompilationUnit()
                .AddUsings(GetStandartNamespaces().ToArray())
                .AddMembers(ns).NormalizeWhitespace();

            return newNode;
        }

        public override Dictionary<string, string> GenerateFilesFromComponent()
        {
            var result = new Dictionary<string, string>();

            result.Add($"Extension{Component.Name}.cs", GenerateExtension().ToString());
            result.Add($"Interface{Component.Name}.cs", GenerateInterface().ToString());

            foreach (var componentObject in GetTypes<PDocumentObjectType>())
            {
                result.Add($"ComponentObject{componentObject.Name}Dto.cs", GenerateDtoClass(componentObject).ToString());
                result.Add($"ComponentObject{componentObject.Name}Entity.cs", GenerateEntityClass(componentObject).ToString());
            }

            return result;
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

            return SyntaxFactory.CompilationUnit()
                .AddUsings(GetStandartNamespaces().ToArray())
                .AddMembers(managerClass).NormalizeWhitespace();

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
        private IEnumerable<UsingDirectiveSyntax> GetStandartNamespaces()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ZenPlatform.Core"));
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ZenPlatform.Core.Annotations"));
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ZenPlatform.DocumentComponent"));
        }

        private AccessorListSyntax GetEmptyAccessor()
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

        private PropertyDeclarationSyntax GetPropertyWithEmptyAccessor(string name, string type)
        {
            return
                SyntaxFactory.PropertyDeclaration(
                        SyntaxFactory.ParseTypeName(type),
                        name).WithAccessorList(GetEmptyAccessor())
                    .WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));
        }

    }
}