using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Primitive;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    /// <summary>
    /// Описание типа компонента
    /// Не забываем также реализовать свойсва, они загружаются в IComponentLoader
    /// </summary>
    public abstract class XCObjectTypeBase : XCTypeBase, IChildItem<XCComponent>
    {
        public XCObjectTypeBase()
        {
        }

        private XCComponent _parent;

        [XmlElement] public bool IsAbstract { get; set; }

        [XmlElement] public bool IsSealed { get; set; }

        [XmlAttribute] public Guid BaseTypeId { get; set; }

        [XmlIgnore] public XCComponent Parent => _parent;

        XCComponent IChildItem<XCComponent>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}