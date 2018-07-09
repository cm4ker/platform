using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using ZenPlatform.Configuration.ConfigurationLoader.Structure.Data;
using ZenPlatform.Configuration.ConfigurationLoader.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.ConfigurationLoader.Structure.Data.Types.Primitive;
using ZenPlatform.Contracts;
using ZenPlatform.Core;
using ZenPlatform.DataComponent;
using ZenPlatform.DataComponent.Entity;
using ZenPlatform.EntityComponent.Configuration;


namespace ZenPlatform.EntityComponent.Entity
{
    public class SingleEntityGenerator : EntityGeneratorBase
    {
        public SingleEntityGenerator(XCComponent component) : base(component)
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

        public override CodeGenRule GetInForeignPropertySetActionRule()
        {
            return new CodeGenRule(CodeGenRuleType.InForeignPropertyGetActionRule,
                "Session.{ComponentName}().{ObjectName}.Load({Params})");
        }

        public override CodeGenRule GetInForeignPropertyGetActionRule()
        {
            return new CodeGenRule(CodeGenRuleType.InForeignPropertySetActionRule,
                "{SetVariable} = Session.{ComponentName}().{ObjectName}.GetKey({Params});");
        }

        public override CodeGenRule GetEntityClassPostfixRule()
        {
            return new CodeGenRule(CodeGenRuleType.EntityClassPostfixRule, "Entity");
        }

        public virtual SyntaxNode GenerateDtoClass(Configuration.XCSingleEntity xcSingleEntity)
        {
            var component = xcSingleEntity.Parent;
            var nsRule = component.GetCodeRule(CodeGenRuleType.NamespaceRule);

            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                workspace, LanguageNames.CSharp);

            var usings = generator.NamespaceImportDeclaration("System");

            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsRule.GetExpression()));

            if (xcSingleEntity.IsAbstract) return null;

            var members = new List<SyntaxNode>();

            foreach (var prop in xcSingleEntity.Properties)
            {
                if (prop.Types.Count == 1)
                {
                    var propType = prop.Types.First();

                    if (propType is XCObjectTypeBase)
                    {
                        members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_Ref", "Guid"));
                    }

                    if (propType is XCPremitiveType primitiveType)
                        members.Add(GetPropertyWithEmptyAccessor(prop.DatabaseColumnName,
                            primitiveType.CLRType.CSharpName()));
                }
                else
                {
                    bool alreadyHaveObjectTypeField = false;

                    foreach (var type in prop.Types)
                    {
                        if (type is XCObjectTypeBase && !alreadyHaveObjectTypeField)
                        {
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_Ref", "Guid"));
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_Type", "int"));

                            alreadyHaveObjectTypeField = true;
                        }

                        if (type is XCPremitiveType primitiveType)
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_{primitiveType.Name}",
                                primitiveType.CLRType.CSharpName()));
                    }
                }
            }

            var classDefinition = generator.ClassDeclaration(
                GetDtoClassName(xcSingleEntity),
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
        /// <param name="xcSingleEntity"></param>
        /// <returns></returns>
        public virtual SyntaxNode GenerateEntityClass(Configuration.XCSingleEntity xcSingleEntity)
        {
            var dtoClassName = GetDtoClassName(xcSingleEntity);


            var nsRule = xcSingleEntity.Parent.GetCodeRule(CodeGenRuleType.NamespaceRule);
            var usings = Generator.NamespaceImportDeclaration("System");
            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsRule.GetExpression()));

            if (xcSingleEntity.IsAbstract) return null;

            var members = new List<SyntaxNode>();

            var dtoPrivateField =
                Generator.FieldDeclaration(DtoPrivateFieldName, SyntaxFactory.ParseTypeName(dtoClassName));


            //TODO: Алгоритм для обхода полей объекта
            /*
                Если поле PrimitiveType, при условии что этот тип один - взять имя свойства как имя поля, взять тип свойства как тип поля
                Если поле ObjectType(Другой объект конфигурации) - Взять и имя свойства, как имя свойста, взять тип, как наименование типа Entity
                                
            */

            //Генерируем свойства
            foreach (var prop in xcSingleEntity.Properties)
            {
                if (prop.Types.Count == 1)
                {
                    members.Add(GenerateEntityClassPropertyOneType(prop));
                }
                else
                {
                    bool alreadyHaveObjectTypeField = false;
                    foreach (var type in prop.Types)
                    {
                        if (type is XCObjectTypeBase && !alreadyHaveObjectTypeField)
                        {
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_Ref", "Guid"));
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_Type", "int"));
                            alreadyHaveObjectTypeField = true;
                        }

                        if (type is XCPremitiveType primitiveType)
                            members.Add(GetPropertyWithEmptyAccessor($"{prop.DatabaseColumnName}_{primitiveType.Name}",
                                primitiveType.CLRType.CSharpName()));
                    }
                }
            }

            //Объявляем конструктор
            var parameterList =
                SyntaxFactory.ParseParameterList($"[NotNull] {nameof(Session)} session, {dtoClassName} dto");
            var statementsParams = parameterList.Parameters.ToArray<SyntaxNode>();
            var baseConstructorArguments = SyntaxFactory.ArgumentList();
            baseConstructorArguments =
                baseConstructorArguments.AddArguments(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("session")));

            var statementsBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement($"{DtoPrivateFieldName} = dto;")
            };

            var constructor = Generator.ConstructorDeclaration(null, statementsParams, Accessibility.Public,
                statements: statementsBody, baseConstructorArguments: baseConstructorArguments.Arguments);


            members.Insert(0, constructor);
            members.Insert(0, dtoPrivateField);

            #region CRUD methods

            var saveBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement(
                    $"Session.{xcSingleEntity.Parent.Info.ComponentSpaceName}().{xcSingleEntity.Name}.Save(this);")
            };
            var saveMethod =
                Generator.MethodDeclaration("Save", statements: saveBody, accessibility: Accessibility.Public);

            var loadBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement(
                    $"var key = Session.{xcSingleEntity.Parent.Info.ComponentSpaceName}().{xcSingleEntity.Name}.GetKey(this);"),
                SyntaxFactory.ParseStatement(
                    $"var entity = Session.{xcSingleEntity.Parent.Info.ComponentSpaceName}().{xcSingleEntity.Name}.Load(key);"),
                SyntaxFactory.ParseStatement($"_dto = entity._dto;"),
            };
            var loadMethod =
                Generator.MethodDeclaration("Load", statements: loadBody, accessibility: Accessibility.Public);

            var deleteBody = new SyntaxNode[]
            {
                SyntaxFactory.ParseStatement(
                    $"Session.{xcSingleEntity.Parent.Info.ComponentSpaceName}().{xcSingleEntity.Name}.Delete(this);")
            };
            var deleteMethod =
                Generator.MethodDeclaration("Delete", statements: deleteBody, accessibility: Accessibility.Public);

            members.Add(saveMethod);
            members.Add(loadMethod);
            members.Add(deleteMethod);

            #endregion

//            var preffix = conf.Parent.GetCodeRule(CodeGenRuleType.EntityClassPrefixRule).GetExpression();
//            var postfix = conf.Parent.GetCodeRule(CodeGenRuleType.EntityClassPostfixRule).GetExpression();

            var classDefinition = Generator.ClassDeclaration(
                GetEntityClassName(xcSingleEntity),
                typeParameters: null,
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Partial,
                baseType: SyntaxFactory.ParseTypeName(nameof(SingleEntity)),
                interfaceTypes: null,
                members: members) as MemberDeclarationSyntax;


            ns = ns.AddMembers(classDefinition);

            var newNode = SyntaxFactory.CompilationUnit()
                .AddUsings(GetStandartNamespaces().ToArray())
                .AddMembers(ns).NormalizeWhitespace();

            return newNode;
        }

        private SyntaxNode GenerateEntityClassPropertyOneType(XCSingleEntityProperty prop)
        {
            var propertyTypeName = string.Empty;

            SyntaxNode[] getAcessorStatement = default(SyntaxNode[]),
                setAcessorStatement = default(SyntaxNode[]);

            SyntaxNode csProperty = null;
            var propType = prop.Types.First();

            if (propType is XCObjectTypeBase objectProprty)
            {
                var propertyComponent = objectProprty.Parent;

                var propEnittyPreffix = propertyComponent.GetCodeRule(CodeGenRuleType.EntityClassPrefixRule)
                    .GetExpression();
                var propEntityPostfix = propertyComponent.GetCodeRule(CodeGenRuleType.EntityClassPostfixRule)
                    .GetExpression();

                var getRule = propertyComponent.GetCodeRule(CodeGenRuleType.InForeignPropertyGetActionRule);
                var setRule = propertyComponent.GetCodeRule(CodeGenRuleType.InForeignPropertySetActionRule);

                var getExp = getRule.GetExpression().NamedFormat(new StandartGetExpressionParameters()
                {
                    ComponentSpace = propertyComponent.Info.ComponentSpaceName,
                    ObjectName = propType.Name,
                    Params = $"{DtoPrivateFieldName}.{prop.DatabaseColumnName}_Ref"
                });

                var setExp = setRule.GetExpression().NamedFormat(new StandartSetExpressionParameters()
                {
                    ComponentSpace = propertyComponent.Info.ComponentSpaceName,
                    SetVariable = $"{DtoPrivateFieldName}.{prop.DatabaseColumnName}_Ref",
                    ObjectName = propType.Name,
                    Params = "value"
                });


                getAcessorStatement = new SyntaxNode[]
                {
                    SyntaxFactory.ParseStatement($"return {getExp};")
                };

                setAcessorStatement = new SyntaxNode[]
                {
                    SyntaxFactory.ParseStatement($"{setExp}"),
                    SyntaxFactory.ParseStatement("OnPropertyChanged();")
                };

                propertyTypeName = $"{propEnittyPreffix}{propType.Name}{propEntityPostfix}";
            }

            if (propType is XCPremitiveType primitiveType)
            {
                getAcessorStatement = new SyntaxNode[]
                {
                    SyntaxFactory.ParseStatement($"return {DtoPrivateFieldName}.{prop.DatabaseColumnName};")
                };
                setAcessorStatement = new SyntaxNode[]
                {
                    SyntaxFactory.ParseStatement($"{DtoPrivateFieldName}.{prop.DatabaseColumnName} = value;"),
                    SyntaxFactory.ParseStatement("OnPropertyChanged();")
                };

                propertyTypeName = primitiveType.CLRType.CSharpName();
            }

            return Generator.PropertyDeclaration(
                prop.Alias,
                SyntaxFactory.IdentifierName(propertyTypeName),
                Accessibility.Public,
                DeclarationModifiers.None, getAcessorStatement, setAcessorStatement);
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
        /// Метод для генерации интерфейса сущностей, привязанных к компоменту
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public SyntaxNode GenerateInterface()
        {
            var workspace = new AdhocWorkspace();

            var generator = SyntaxGenerator.GetGenerator(
                workspace, LanguageNames.CSharp);

            var nsRule = Component.GetCodeRule(CodeGenRuleType.NamespaceRule);
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

            var preffix = Component.GetCodeRule(CodeGenRuleType.EntityClassPrefixRule);
            var postfix = Component.GetCodeRule(CodeGenRuleType.EntityClassPostfixRule);

            foreach (var obj in Component.Types)
            {
                var getBody = SyntaxFactory.Block(
                    SyntaxFactory.ParseStatement(
                        $"return new DocumentEntityManager<{preffix.GetExpression()}{obj.Name}{postfix.GetExpression()}>(Session);")
                );
                var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, getBody);

                var componentProperty = SyntaxFactory.PropertyDeclaration(
                        SyntaxFactory.ParseTypeName(
                            $"DocumentEntityManager<{preffix.GetExpression()}{obj.Name}{postfix.GetExpression()}>"),
                        obj.Name).WithAccessorList(SyntaxFactory.AccessorList().AddAccessors(getAccessor))
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

            var nsRule = Component.GetCodeRule(CodeGenRuleType.NamespaceRule);
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

            result.Add($"Extension{Component.Info.ComponentSpaceName}.cs", GenerateExtension().ToString());
            result.Add($"Interface{Component.Info.ComponentSpaceName}.cs", GenerateInterface().ToString());

            foreach (var obj in Component.Types)
            {
                result.Add($"ComponentObject{obj.Name}Dto.cs",
                    GenerateDtoClass(obj as Configuration.XCSingleEntity).ToString());
                result.Add($"ComponentObject{obj.Name}Entity.cs",
                    GenerateEntityClass(obj as Configuration.XCSingleEntity).ToString());
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

            return generator.CompilationUnit(managerClass).NormalizeWhitespace();
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