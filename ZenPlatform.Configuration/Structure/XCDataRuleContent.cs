﻿using System;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    /// <summary>
    /// Контент, содержащий в себе правила.
    /// Служит премежуточным звеном между правилами компонента и конфигурацией
    ///
    /// Это сделано для того, чтобы абстрагировать правила от компонента и предоставить единый интерфейс
    /// </summary>
    public class XCDataRuleContent : IChildItem<XCRole>
    {
        private XCRole _parent;
        private string _content;

        /// <summary>
        /// Идентификатор объекта конфигурации базы данных
        /// </summary>
        [XmlAttribute] public Guid ObjectId { get; set; }

        /// <summary>
        /// Содержание xml-представления роли для объекта
        /// Так как роли у компонентов разные и они не имеют ничего общего по структуре - они
        /// просто реализуются внутри компоннета, унаследуя класс XCDataRuleBase
        /// </summary>
        [XmlElement]
        public string Content
        {
            get => Rule?.Serialize();
            set { _content = value; }
        }

        [XmlIgnore] public string RealContent => _content;

        /// <summary>
        /// Привязанные правила к контенту
        /// </summary>
        [XmlIgnore]
        public XCDataRuleBase Rule { get; private set; }

        /// <summary>
        /// Объект к которому принадлежит правило
        /// </summary>
        [XmlIgnore]
        public XCObjectTypeBase Object =>
            Role.Root.Data.PlatformTypes.First(x => x.Guid == ObjectId) as XCObjectTypeBase;

        [XmlIgnore] public XCRole Role => _parent;

        [XmlIgnore]
        XCRole IChildItem<XCRole>.Parent
        {
            get => _parent;
            set { _parent = value; }
        }

        public void Load()
        {
            Rule = Object.Parent.Loader.LoadRule(this);
        }
    }
}