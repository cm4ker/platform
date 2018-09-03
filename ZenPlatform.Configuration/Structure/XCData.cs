﻿using System;
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
        private List<XCTypeBase> _platformTypes;
        public XCData()
        {
            Components = new ChildItemCollection<XCData, XCComponent>(this);
            _platformTypes = new List<XCTypeBase>();

            //Инициализируем примитивные типы платформы, они нужны для правильного построения зависимостей
            _platformTypes.Add(new XCBinary());
            _platformTypes.Add(new XCString());
            _platformTypes.Add(new XCDateTime());
            _platformTypes.Add(new XCBoolean());
            _platformTypes.Add(new XCNumeric());
            _platformTypes.Add(new XCGuid());
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
                    component.AttachedComponents.Add(Components.First(x =>
                        x.Info.ComponentId == attachedComponentId && x.Info.AttachedComponent));
                }
            }

            //Загружаем зависимости типов
            foreach (var xct in ComponentTypes)
            {
                xct.LoadDependencies();
            }
        }

        #endregion

        #region Saving

        /// <summary>
        /// Созрание всех данных из раздела Data
        /// </summary>
        public void Save()
        {
            foreach (var component in Components)
            {
                component.SaveComponent();
            }
        }

        #endregion

        [XmlIgnore] public IEnumerable<XCTypeBase> PlatformTypes => _platformTypes;

        [XmlIgnore]
        public IEnumerable<XCObjectTypeBase> ComponentTypes =>
            PlatformTypes.Where(x => x is XCObjectTypeBase).Cast<XCObjectTypeBase>();

        [XmlIgnore] public XCRoot Parent => _parent;


        XCRoot IChildItem<XCRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }


        public void RegisterType(XCObjectTypeBase type)
        {
            _platformTypes.Add(type);
        }
    }

    internal class TypeNotFoundException : Exception
    {
    }

    public class ComponentNotFoundException : Exception
    {
    }
}