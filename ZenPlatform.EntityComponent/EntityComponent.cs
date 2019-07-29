using System.Collections.Generic;
using System.Runtime.Caching;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.DataComponent;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.EntityComponent.Entity;
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
            Generator = new SingleEntityGenerator(Component);
            Manager = new SingleEntityManager();
            ComponentManager = new SingleEntityConfigurationManager(Component);

            //TODO: Вынести интерфейс генерации UI в DataComponentBase. Если мы взаимодействуем с данными, то мы должны их как-то показывать
            InterfaceGenerator = new InterfaceGenerator();

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
            Component.RegisterCodeRule(Generator.GetInForeignPropertySetActionRule());
            Component.RegisterCodeRule(Generator.GetInForeignPropertyGetActionRule());
            Component.RegisterCodeRule(Generator.GetEntityClassPostfixRule());
            Component.RegisterCodeRule(Generator.GetEntityClassPrefixRule());
            Component.RegisterCodeRule(Generator.GetNamespaceRule());
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
}