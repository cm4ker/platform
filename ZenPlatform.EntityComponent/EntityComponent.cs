using System.Collections.Generic;
using System.Runtime.Caching;
using System.Security.Policy;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.DataComponent;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.EntityComponent.Migrations;
using ZenPlatform.EntityComponent.QueryBuilders;
using ZenPlatform.EntityComponent.UIGenerations;

namespace ZenPlatform.EntityComponent
{
    public class EntityComponent : DataComponentBase
    {
        public EntityComponent(XCComponent component) : base(component)
        {
        }

        public override void OnInitializing()
        {
            //Generator = new StagedGenerator(Component);
            Manager = new SingleEntityManager();
            ComponentManager = new SingleEntityConfigurationManager(Component);

            //TODO: Вынести интерфейс генерации UI в DataComponentBase. Если мы взаимодействуем с данными, то мы должны их как-то показывать
            InterfaceGenerator = new InterfaceGenerator();
            DatabaseObjectsGenerator = new EntityDatabaseObjectGenerator();

            QueryInjector = new SingleEntityQueryInjector(Component);

            Generator = new EntityPlatformGenerator(Component); // new StagedGeneratorAst(Component);

            Migrator = new SingleEntityMigrator();

            RegisterSupportedTypes();
            RegisterCodeRules();

            base.OnInitializing();
        }

        public InterfaceGenerator InterfaceGenerator { get; private set; }

        private void RegisterSupportedTypes()
        {
            //Выпилено 
            // SupportedTypesManager.RegisterType(typeof(PDocumentObjectType));
        }

        private void RegisterCodeRules()
        {
            Component.RegisterCodeRule(new CodeGenRule(CodeGenRuleType.DtoPostfixRule, ""));
            Component.RegisterCodeRule(new CodeGenRule(CodeGenRuleType.DtoPreffixRule, "_"));
            Component.RegisterCodeRule(new CodeGenRule(CodeGenRuleType.EntityClassPostfixRule, ""));
            Component.RegisterCodeRule(new CodeGenRule(CodeGenRuleType.EntityClassPrefixRule, ""));
            Component.RegisterCodeRule(new CodeGenRule(CodeGenRuleType.NamespaceRule, "Documents"));
        }

        /*
         * Компилятор запросов
         * Должен быть свой для каждого компонента
         * Компилятор запросов должен покрывать все объекты любой сложности, которые могут возникнуть внутри компонента
         *
         * Для примера есть стандартные компоненты компилятора запросов, они необходимы для того, чтобы упростить создание комопнента который построен на более-менее стандартных принципах
         * Вопрос, а нужны ли эти стандартные компоненты?
         *
         * Для документа этот компонент толжен выполнять следующую задачу:
         *  1) Вставка нового документа
         *  2) Обновление существующего документа
         *  3) Удаление документа (как мягкое так и из БД) - здесь важно не забыть что мягкое удаление влияяет на Выборку документов
         *  4) Чтение докуменота - Здесь нужно продумать очень гибкую архитектуру - документ можно считать, но у пользователя может не быть на него разрешения.
         *   
         *
         */
    }

    public class EntityDatabaseObjectGenerator : IDatabaseObjectsGenerator
    {
        public Dictionary<string, XCPrimitiveType> GetColumnOptions()
        {
            throw new System.NotImplementedException();
        }

        public bool HasForeignColumn => true;
    }
}