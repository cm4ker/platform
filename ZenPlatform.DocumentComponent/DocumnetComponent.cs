using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.SimpleRealization;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.Core.Entity;
using ZenPlatform.CSharpCodeBuilder.Syntax;
using ZenPlatform.DataComponent;
using ZenPlatform.DocumentComponent.Migrations;


namespace ZenPlatform.DocumentComponent
{
    public class DocumnetComponent : DataComponentBase<DocumentComponentMigration, PDocumentObjectType, PDocumentComplexObjectType, DocumentEntityGenerator, DocumentManager>
    {
        public DocumnetComponent(PComponent component) : base(component)
        {
            Generator = new DocumentEntityGenerator();
            Manager = new DocumentManager();

            RegisterCodeRules();
        }

        private void RegisterCodeRules()
        {
            Component.RegisterCodeRule(Generator.GetInForeignPropertySetActionRule());
            Component.RegisterCodeRule(Generator.GetInForeignPropertyGetActionRule());
            Component.RegisterCodeRule(Generator.GetEntityClassPostfixRule());
            Component.RegisterCodeRule(Generator.GetEntityClassPrefixRule());

            Component.RegisterCodeRule(Generator.GetNamespaceRule());
        }

        public override EntityManagerBase Manager { get; }
        public override EntityGeneratorBase Generator { get; }

        public override DataComponentObject ConfigurationUnloadHandler(PObjectType pobject)
        {
            var documentObject = pobject as PSimpleObjectType ?? throw new NullReferenceException("Can't transform PObjectType into PSimpleObjectType or the argument is null");

            var result = new DataComponentObject();

            result.Name = documentObject.Name;
            result.ComponentId = documentObject.OwnerComponent.Id;
            result.Id = documentObject.Id;

            foreach (var prop in documentObject.Properties)
            {
                var types = prop.Types.Select(x => x.Id).ToList();
                result.Properties.Add(new DataComponentObjectProperty()
                {
                    Id = prop.Id,
                    Types = types,
                    Name = prop.Name,
                    Alias = prop.Alias,
                    Unique = prop.Unique

                });
            }
            return result;
        }
        public override PObjectType ConfigurationComponentObjectLoadHandler(PComponent component, DataComponentObject componentObject)
        {
            var pobject = component.CreateObject<PSimpleObjectType>(componentObject.Name, componentObject.Id);
            pobject.Description = componentObject.Description;

            return pobject;
        }
        public override void ConfigurationObjectPropertyLoadHandler(PObjectType pObject, DataComponentObjectProperty objectProperty, List<PTypeBase> registeredTypes)
        {
            if (objectProperty.Unique) return;

            var pproperty = new PSimpleProperty(pObject);
            pproperty.Name = objectProperty.Name;
            pproperty.Alias = objectProperty.Alias;
            pproperty.Id = objectProperty.Id;

            foreach (var typeId in objectProperty.Types)
            {
                var ptype = registeredTypes.FirstOrDefault(x => x.Id == typeId) ?? throw new Exception($"Type not found: {typeId}");
                pproperty.Types.Add(ptype);
            }

            pObject.Properties.Add(pproperty);
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
