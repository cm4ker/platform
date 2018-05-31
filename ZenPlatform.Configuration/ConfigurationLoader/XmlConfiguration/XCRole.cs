﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    [XmlRoot("Role")]
    public class XCRole : IChildItem<XmlConfRoles>
    {
        private XmlConfRoles _parent;
        private XCPlatformRule _platformRules;

        public XCRole()
        {
            DataRules = new ChildItemCollection<XCRole, XCDataRuleContent>(this);
        }

        [XmlElement] public Guid Id { get; set; }

        [XmlElement] public string Name { get; set; }

        [XmlElement]
        public XCPlatformRule PlatformRules
        {
            get => _platformRules;
            set
            {
                _platformRules = value;
                ((IChildItem<XCRole>) _platformRules).Parent = this;
            }
        }

        [XmlArray]
        [XmlArrayItem(ElementName = "Rule")]
        public ChildItemCollection<XCRole, XCDataRuleContent> DataRules { get; }

        [XmlIgnore] public XmlConfRoles Roles => _parent;
        [XmlIgnore] public XCRoot Root => _parent.Parent;

        XmlConfRoles IChildItem<XmlConfRoles>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        internal void Load()
        {
            foreach (var dataRule in DataRules)
            {
                dataRule.Load();
            }
        }
    }

    /// <summary>
    /// Контент, содержащий в себе правила.
    /// Служит премежуточным звеном между правилами компонента и конфигурацией
    ///
    /// Это сделано для того, чтобы абстрогировать правила от компонента и предоставить единый интерфейс
    /// </summary>
    public class XCDataRuleContent : IChildItem<XCRole>
    {
        private XCRole _parent;
        private string _content;

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
        /// Привязанная роль к контенту
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

    public class XCDataRuleBase : IChildItem<XCDataRuleContent>
    {
        private XCDataRuleContent _parent;

        public XCDataRuleBase()
        {
        }

        public XCDataRuleContent Parent => _parent;

        XCDataRuleContent IChildItem<XCDataRuleContent>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}