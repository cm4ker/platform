using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Primitive;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    [Serializable]
    public class XmlConfData : IChildItem<XmlConfRoot>
    {
        private XmlConfRoot _parent;

        public XmlConfData()
        {
            IncludedFiles = new List<XmlConfFile>();
            Components = new ChildItemCollection<XmlConfData, XCComponent>(this);
            PlatformTypes = new List<XCTypeBase>();

            //Инициализируем типы, они нужны для правильного построения зависимостей
            PlatformTypes.Add(new XCBinary());
            PlatformTypes.Add(new XCString());
            PlatformTypes.Add(new XCDateTime());
            PlatformTypes.Add(new XCBoolean());
            PlatformTypes.Add(new XCNumeric());
        }

        [XmlArray("IncludedFiles")]
        [XmlArrayItem(ElementName = "File", Type = typeof(XmlConfFile))]
        public List<XmlConfFile> IncludedFiles { get; set; }

        [XmlArray("Components")]
        [XmlArrayItem(ElementName = "Component", Type = typeof(XCComponent))]
        public ChildItemCollection<XmlConfData, XCComponent> Components { get; }


        public void Load()
        {
            LoadComponents();
            LoadFiles();
            LoadRules();
        }

        #region Loading

        private void LoadComponents()
        {
            foreach (var component in Components.Where(x => !x.IsLoaded))
            {
                component.LoadComponent();
            }
        }

        private void LoadFiles()
        {
            foreach (var includedFile in IncludedFiles)
            {
                var component = Components.FirstOrDefault(x => x.Info.ComponentId == includedFile.ComponentId);

                if (component is null)
                    throw new ComponentNotFoundException();

                var componentType =
                    component.Loader.LoadComponentType(Path.Combine(XmlConfHelper.BaseDirectory, includedFile.Path),
                        component);

                ((IChildItem<XCComponent>) componentType).Parent = component;

                PlatformTypes.Add(componentType);
            }

            foreach (var xct in ComponentTypes)
            {
                xct.Parent.Loader.CheckDependencies(xct);
            }
        }

        private void LoadRules()
        {
            foreach (var role in Parent.Roles.Items)
            {
                foreach (var rule in role.DataRules)
                {
                    var type = ComponentTypes.FirstOrDefault(x => x.Id == rule.ObjectId);
                    if (type is null)
                        throw new TypeNotFoundException();

                    //так как с правила объекта обрабатывает сам компонент, то нет смысла их выносить наружу.
                    //мы лишь только дадим возможность компоненту загрузить эти правила в объект
                    type.Parent.Loader.LoadTypeRule(type, rule.Content);
                }
            }
        }

        #endregion

        [XmlIgnore] public List<XCTypeBase> PlatformTypes { get; }

        [XmlIgnore]
        public IEnumerable<XCObjectTypeBase> ComponentTypes =>
            PlatformTypes.Where(x => x is XCObjectTypeBase).Cast<XCObjectTypeBase>();

        [XmlIgnore] public XmlConfRoot Parent => _parent;


        XmlConfRoot IChildItem<XmlConfRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }

    internal class TypeNotFoundException : Exception
    {
    }

    public class ComponentNotFoundException : Exception
    {
    }
}