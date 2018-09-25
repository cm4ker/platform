using System;
using System.Xml.Serialization;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    [XmlRoot("Role")]
    public class XCRole : IChildItem<XCRoles>
    {
        private XCRoles _parent;
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
                ((IChildItem<XCRole>)_platformRules).Parent = this;
            }
        }

        [XmlArray]
        [XmlArrayItem(ElementName = "Rule")]
        public ChildItemCollection<XCRole, XCDataRuleContent> DataRules { get; }

        [XmlIgnore] public XCRoles Roles => _parent;
        [XmlIgnore] public XCRoot Root => _parent.Parent;

        XCRoles IChildItem<XCRoles>.Parent
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
}