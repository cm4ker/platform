using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Primitive;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    [Serializable]
    public class XCData : IChildItem<XCRoot>
    {
        private XCRoot _parent;

        public XCData()
        {
            IncludedFiles = new List<XmlConfFile>();
            Components = new ChildItemCollection<XCData, XCComponent>(this);
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
        public ChildItemCollection<XCData, XCComponent> Components { get; }


        public void Load()
        {
            LoadComponents();
            LoadFiles();
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
                    component.Loader.LoadObject(Path.Combine(XmlConfHelper.BaseDirectory, includedFile.Path));
                ((IChildItem<XCComponent>) componentType).Parent = component;

                PlatformTypes.Add(componentType);

                componentType.Initialize();
            }

            foreach (var xct in ComponentTypes)
            {
                xct.LoadDependencies();
            }
        }

        #endregion

        [XmlIgnore] public List<XCTypeBase> PlatformTypes { get; }

        [XmlIgnore]
        public IEnumerable<XCObjectTypeBase> ComponentTypes =>
            PlatformTypes.Where(x => x is XCObjectTypeBase).Cast<XCObjectTypeBase>();

        [XmlIgnore] public XCRoot Parent => _parent;


        XCRoot IChildItem<XCRoot>.Parent
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