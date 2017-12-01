using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.SimpleRealization;
using ZenPlatform.ConfigurationDataComponent;
using ZenPlatform.Core.Entity;
using ZenPlatform.CSharpCodeBuilder.Syntax;
using ZenPlatform.DataComponent;


namespace ZenPlatform.DocumentComponent
{
    public class DocumnetComponent : DataComponentBase
    {
        public DocumnetComponent(PComponent component) : base(component)
        {
            Generator = new DocumentEntityGenerator();
            ObjectConfigurationType = typeof(PSimpleObjectType);
            Manager = new DocumentManager();
            Initialization();
        }

        private void Initialization()
        {
            Component.RegisterCodeRule(Generator.GetInForeignPropertySetActionRule());
            Component.RegisterCodeRule(Generator.GetInForeignPropertyGetActionRule());
            Component.RegisterCodeRule(Generator.GetEntityClassPostfixRule());
            Component.RegisterCodeRule(Generator.GetEntityClassPrefixRule());

            Component.RegisterCodeRule(Generator.GetNamespaceRule());
        }

        public override Type ObjectConfigurationType { get; }
        public override EntityManagerBase Manager { get; }
        public override EntityGeneratorBase Generator { get; }

        public override DataComponentObject ConfigurationUnLoadHandler(PObjectType pobject)
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

        public override void ConfigurationComponentObjectLoadHandler(PComponent component, DataComponentObject componentObject)
        {
            var pobject = component.CreateObject<PSimpleObjectType>(componentObject.Name, componentObject.Id);
            pobject.Description = componentObject.Description;
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
    }
}
