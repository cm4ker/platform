using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    [Serializable]
    public class XCData : IChildItem<XCRoot>
    {
        private XCRoot _parent;

        public XCData()
        {
            Components = new ChildItemCollection<XCData, XCComponent>(this);
            PlatformTypes = new List<XCTypeBase>();

            //Инициализируем примитивные типы платформы, они нужны для правильного построения зависимостей
            PlatformTypes.Add(new XCBinary());
            PlatformTypes.Add(new XCString());
            PlatformTypes.Add(new XCDateTime());
            PlatformTypes.Add(new XCBoolean());
            PlatformTypes.Add(new XCNumeric());
            PlatformTypes.Add(new XCGuid());
        }

        [XmlArray("Components")]
        [XmlArrayItem(ElementName = "Component", Type = typeof(XCComponent))]
        public ChildItemCollection<XCData, XCComponent> Components { get; }

        /// <summary>
        /// Загрузить дерективу и все зависимости
        /// </summary>
        public void Load()
        {
            LoadComponents();
            LoadDependencies();
        }

        #region Loading

        private void LoadComponents()
        {
            foreach (var component in Components.Where(x => !x.IsLoaded))
            {
                component.LoadComponent();
            }
        }

        /// <summary>
        /// Загрузить файлы, относящиеся к разделу Data в дерективе IncludedFiles
        /// </summary>
        private void LoadDependencies()
        {
            //Загружаем присоединённые компоненты, чтобы можно было корректно генерировать зависимости
            foreach (var component in Components)
            {
                foreach (var attachedComponentId in component.AttachedComponentIds)
                {
                    component.AttachedComponents.Add(Components.First(x => x.Info.ComponentId == attachedComponentId && x.Info.AttachedComponent));
                }
            }

            //Загружаем зависимости типов
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