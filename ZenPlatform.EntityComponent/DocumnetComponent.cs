using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.Caching;
using ZenPlatform.Configuration.ConfigurationLoader.Structure.Data;
using ZenPlatform.Core;
using ZenPlatform.DataComponent;
using ZenPlatform.EntityComponent.Entity;

namespace ZenPlatform.EntityComponent
{
    public class DocumnetComponent : DataComponentBase
    {
        public DocumnetComponent(XCComponent component) : base(component)
        {
        }

        public override void OnInitializing()
        {
            Generator = new SingleEntityGenerator(Component);
            Manager = new SingleEntityManager();


            Caches = new Dictionary<string, ObjectCache>();

            RegisterSupportedTypes();
            RegisterCodeRules();
            base.OnInitializing();
        }

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

    public delegate int SomeDelegate();
}