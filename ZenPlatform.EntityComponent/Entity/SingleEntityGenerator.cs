using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using SharpDX.WIC;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Contracts;
using ZenPlatform.Core;
using ZenPlatform.Core.Annotations;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Core.Sessions;
using ZenPlatform.DataComponent;
using ZenPlatform.DataComponent.Entity;
using ZenPlatform.DataComponent.Helpers;
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

        public virtual SyntaxNode GenerateDtoClass(XCObjectTypeBase xcSingleEntity)
        {
            var component = xcSingleEntity.Parent;
            var nsRule = component.GetCodeRule(CodeGenRuleType.NamespaceRule);

            var usings = Generator.NamespaceImportDeclaration("System");

            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsRule.GetExpression()));

            if (xcSingleEntity.IsAbstract) return null;

            var members = new List<SyntaxNode>();

            foreach (var prop in xcSingleEntity.GetProperties())
            {
                foreach (var col in prop.GetColumnsFromProperty())
                {
                    members.Add(GetPropertyWithEmptyAccessor(col.DatabaseColumnName, col.Type.CLRType.ToTypeString()));
                }
            }

            var classDefinition = Generator.ClassDeclaration(
                GetDtoClassName(xcSingleEntity),
                typeParameters: null,
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.None,
                baseType: null,
                interfaceTypes: null,
                members: members) as MemberDeclarationSyntax;


            ns = ns.AddMembers(classDefinition);

            var newNode = SyntaxFactory.CompilationUnit()
                .AddUsings(GetStandardNamespaces().ToArray())
                .AddMembers(ns).NormalizeWhitespace();
            return newNode;
        }

        /// <summary>
        /// Генерация сущности - класс который является промежуточным между DTO и пользователем
        /// </summary>
        /// <param name="xcSingleEntity"></param>
        /// <returns></returns>
        public virtual SyntaxNode GenerateEntityClass(XCObjectTypeBase xcSingleEntity)
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

            var constructorMdsStatements = new List<SyntaxNode>();

            //Генерируем свойства для класса entity
            foreach (var prop in xcSingleEntity.GetProperties())
            {
                foreach (var col in prop.GetColumnsFromProperty().GroupBy(x => x.Property))
                {
                    SyntaxNode property;

                    if (col.Count() > 1)
                    {
                        var getSt = SyntaxFactory.Block(
                            SyntaxFactory.ParseStatement($"return {GetMultiDataStoragePrivateFieldName(col.Key)};"));

                        property = SyntaxFactory.PropertyDeclaration(
                                SyntaxFactory.ParseTypeName(GetMultiDataStorageClassName(col.Key)), col.Key.Name)
                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                            .WithAccessorList(
                                SyntaxFactory.AccessorList().AddAccessors(
                                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                        .WithBody(getSt)));

                        constructorMdsStatements.Add(SyntaxFactory.ParseStatement(
                            $"{GetMultiDataStoragePrivateFieldName(col.Key)} = new {GetMultiDataStorageClassName(col.Key)}({DtoPrivateFieldName});"));

                        members.Add(Generator.FieldDeclaration(GetMultiDataStoragePrivateFieldName(col.Key),
                            SyntaxFactory.ParseTypeName(GetMultiDataStorageClassName(col.Key))));
                    }
                    else
                    {
                        var fcol = col.First();

                        var getSt = SyntaxFactory.Block(
                            SyntaxFactory.ParseStatement($"return {DtoPrivateFieldName}.{fcol.DatabaseColumnName};"));

                        var setSt = SyntaxFactory.Block(
                            SyntaxFactory.ParseStatement($"{DtoPrivateFieldName}.{fcol.DatabaseColumnName} = value;"));

                        property = Generator.PropertyDeclaration(
                            col.Key.Name,
                            SyntaxFactory.ParseTypeName(fcol.Type.CLRType.ToTypeString()),
                            Accessibility.Public,
                            getAccessorStatements: getSt.Statements,
                            setAccessorStatements: setSt.Statements);
                    }

                    members.Add(property);
                }
            }

            //Объявляем конструктор
            var parameterList =
                SyntaxFactory.ParseParameterList($"[NotNull] {nameof(UserSession)} session, {dtoClassName} dto");
            var statementsParams = parameterList.Parameters.ToArray<SyntaxNode>();
            var baseConstructorArguments = SyntaxFactory.ArgumentList();
            baseConstructorArguments =
                baseConstructorArguments.AddArguments(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("session")));

            var statementsBody = new List<SyntaxNode>()
            {
                SyntaxFactory.ParseStatement($"{DtoPrivateFieldName} = dto;")
            };

            statementsBody.AddRange(constructorMdsStatements);

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
                .AddUsings(GetStandardNamespaces().ToArray())
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

                propertyTypeName = primitiveType.CLRType.ToTypeString();
            }

            return Generator.PropertyDeclaration(
                prop.Name,
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
            var nsRule = Component.GetCodeRule(CodeGenRuleType.NamespaceRule);
            var systemUsing = Generator.NamespaceImportDeclaration("System");
            var coreUsing = Generator.NamespaceImportDeclaration(nameof(ZenPlatform.Core));


            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsRule.GetExpression()));


            var constructorBody = SyntaxFactory.Block(SyntaxFactory.ParseStatement("Session = session;"));
            var parameters = SyntaxFactory.ParseParameterList($"{nameof(UserSession)} session");

            var constructor = Generator.ConstructorDeclaration($"{Component.Info.ComponentName}Interface",
                parameters.Parameters,
                statements: constructorBody.Statements,
                accessibility: Accessibility.Public);

            var sessionProperty =
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(nameof(UserSession)), "Session")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                    .WithAccessorList(
                        SyntaxFactory.AccessorList().AddAccessors(
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))));

            var interfaceClass = SyntaxFactory.ClassDeclaration($"{Component.Info.ComponentName}Interface")
                .AddMembers(constructor as MemberDeclarationSyntax)
                .AddMembers(sessionProperty)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var preffix = Component.GetCodeRule(CodeGenRuleType.EntityClassPrefixRule);
            var postfix = Component.GetCodeRule(CodeGenRuleType.EntityClassPostfixRule);

            foreach (var obj in Component.Types)
            {
                var getBody = SyntaxFactory.Block(
                    SyntaxFactory.ParseStatement(
                        $"return new {Component.Info.ComponentName}EntityManager<{preffix.GetExpression()}{obj.Name}{postfix.GetExpression()}>(Session);")
                );
                var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, getBody);

                var componentProperty = SyntaxFactory.PropertyDeclaration(
                        SyntaxFactory.ParseTypeName(
                            $"{Component.Info.ComponentName}EntityManager<{preffix.GetExpression()}{obj.Name}{postfix.GetExpression()}>"),
                        obj.Name).WithAccessorList(SyntaxFactory.AccessorList().AddAccessors(getAccessor))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                interfaceClass = interfaceClass.AddMembers(componentProperty);
            }

            ns = ns.AddMembers(interfaceClass);

            var newNode = SyntaxFactory.CompilationUnit()
                .AddUsings(GetStandardNamespaces().ToArray())
                .AddMembers(ns).NormalizeWhitespace();

            return newNode;
        }

        /// <summary>
        /// Сгенерировать класс для хранения свойства, которое может принимать больше чем одно значение
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public SyntaxNode GenerateMultiDataStorage(XCObjectTypeBase parent, XCObjectPropertyBase property)
        {
            var members = new List<SyntaxNode>();
            var nsRule = Component.GetCodeRule(CodeGenRuleType.NamespaceRule);
            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsRule.GetExpression()));

            var constructorBody = SyntaxFactory.Block(SyntaxFactory.ParseStatement($"{DtoPrivateFieldName} = dto;"));
            var parameters = SyntaxFactory.ParseParameterList($"{GetDtoClassName(parent)} dto");

            var constructor = Generator.ConstructorDeclaration(GetMultiDataStorageClassName(property),
                parameters.Parameters,
                statements: constructorBody.Statements,
                accessibility: Accessibility.Internal);


            //Start value property
            var getBody = SyntaxFactory.Block(SyntaxFactory.ParseStatement($"return Get();"));
            var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, getBody);

            var valueProperty = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("object"), "Value")
                .WithAccessorList(SyntaxFactory.AccessorList().AddAccessors(getAccessor))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            //End value property

            //Start Get() method
            var getMethodBody = SyntaxFactory.Block();
            var getMethodWithType = SyntaxFactory.Block();
            var clearAllMethodBody = SyntaxFactory.Block();
            var setsMethods = new List<MethodDeclarationSyntax>();

            foreach (var columnDefinitionItem in property.GetColumnsFromProperty())
            {
                getMethodBody = getMethodBody.AddStatements(SyntaxFactory.ParseStatement(
                    $"if ({DtoPrivateFieldName}.{columnDefinitionItem.DatabaseColumnName} != default({columnDefinitionItem.Type.CLRType.ToTypeString()})) return {DtoPrivateFieldName}.{columnDefinitionItem.DatabaseColumnName};"));
                
                clearAllMethodBody = clearAllMethodBody.AddStatements(SyntaxFactory.ParseStatement(
                    $"{DtoPrivateFieldName}.{columnDefinitionItem.DatabaseColumnName} = default({columnDefinitionItem.Type.CLRType.ToTypeString()});"));

                var setMethodBody = SyntaxFactory.Block()
                    .AddStatements(SyntaxFactory.ParseStatement("ClearAll();"))
                    .AddStatements(
                        SyntaxFactory.ParseStatement(
                            $"{DtoPrivateFieldName}.{columnDefinitionItem.DatabaseColumnName} = value;"));

                var setParamList =
                    SyntaxFactory.ParseParameterList($"{columnDefinitionItem.Type.CLRType.ToTypeString()} value");

                var setMethod = Generator.MethodDeclaration(
                    "Set",
                    setParamList.Parameters,
                    accessibility: Accessibility.Public,
                    statements: setMethodBody.Statements);


                setsMethods.Add(setMethod as MethodDeclarationSyntax);
            }

            getMethodBody = getMethodBody.AddStatements(SyntaxFactory.ParseStatement("return null"));

            var getMethod = Generator.MethodDeclaration(
                "Get",
                returnType: SyntaxFactory.ParseTypeName("object"),
                statements: getMethodBody.Statements,
                accessibility: Accessibility.Public);
            
            var clearAllMethod = Generator.MethodDeclaration(
                "ClearAll",
                statements: clearAllMethodBody.Statements,
                accessibility: Accessibility.Private);

            var hasValueMethod = Generator.MethodDeclaration(
                "HasValue",
                returnType: SyntaxFactory.ParseTypeName("bool"),
                statements: SyntaxFactory.Block(SyntaxFactory.ParseStatement("return Get() == null;"))
                    .Statements,
                accessibility: Accessibility.Public) as MethodDeclarationSyntax;

            var clearMethod = Generator.MethodDeclaration(
                "Clear",
                statements: SyntaxFactory.Block(SyntaxFactory.ParseStatement("ClearAll();"))
                    .Statements,
                accessibility: Accessibility.Public) as MethodDeclarationSyntax;

            //End Get() method

            var multidataStorageClass = SyntaxFactory.ClassDeclaration(GetMultiDataStorageClassName(property))
                .AddMembers(constructor as MemberDeclarationSyntax)
                .AddMembers(valueProperty)
                .AddMembers(hasValueMethod)
                .AddMembers(getMethod as MethodDeclarationSyntax)
                .AddMembers(clearAllMethod as MethodDeclarationSyntax)
                .AddMembers(clearMethod)
                .AddMembers(setsMethods.ToArray())
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));


            ns = ns.AddMembers(multidataStorageClass);

            var newNode = SyntaxFactory.CompilationUnit()
                .AddUsings(GetStandardNamespaces().ToArray())
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
            var nsRule = Component.GetCodeRule(CodeGenRuleType.NamespaceRule);
            var usings = Generator.NamespaceImportDeclaration("System");
            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsRule.GetExpression()));


            var body = SyntaxFactory.Block(
                SyntaxFactory.ParseStatement($"return new {Component.Info.ComponentName}Interface(session);"));
            var parameters = SyntaxFactory.ParseParameterList("this UserSession session");

            var method = Generator.MethodDeclaration(Component.Info.ComponentName,
                parameters.Parameters,
                statements: body.Statements,
                returnType: SyntaxFactory.ParseTypeName($"{Component.Info.ComponentName}Interface"),
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Static);

            var staticClass = SyntaxFactory.ClassDeclaration($"{Component.Info.ComponentName}ComponentSessionExtension")
                .AddMembers(method as MemberDeclarationSyntax)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));

            ns = ns.AddMembers(staticClass);

            var newNode = SyntaxFactory.CompilationUnit()
                .AddUsings(GetStandardNamespaces().ToArray())
                .AddMembers(ns).NormalizeWhitespace();

            return newNode;
        }

        public override Dictionary<string, string> GenerateSourceFiles()
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
              Session.Document().Invoice.Save();
              Session.Register().Incommings.GetRange();
              Session.Reference().Nomenclature.Create();
         */

        public SyntaxNode GenerateHelpersForEntity()
        {
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
                .AddUsings(GetStandardNamespaces().ToArray())
                .AddMembers(managerClass).NormalizeWhitespace();
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
        private IEnumerable<UsingDirectiveSyntax> GetStandardNamespaces()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(ISession).Namespace));
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(CanBeNullAttribute).Namespace));
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(EntityComponent).Namespace));
        }

        /// <summary>
        /// Пулчить пустые методы доступа
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Получить свойство без методов
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
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