using System;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{

    [XmlRoot("Role")]
    public class XCRole : IXCRole
    {
        private IXCRoles _parent;
        private IXCPlatformRule _platformRules;

        public XCRole()
        {
            DataRules = new ChildItemCollection<IXCRole, IXCDataRuleContent>(this);
        }

        [XmlElement] public Guid Id { get; set; }

        [XmlElement] public string Name { get; set; }

        [XmlElement]
        public IXCPlatformRule PlatformRules
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
        public ChildItemCollection<IXCRole, IXCDataRuleContent> DataRules { get; }

        [XmlIgnore] public IXCRoles Roles => _parent;
        [XmlIgnore] public IXCRoot Root => _parent.Parent;

        IXCRoles IChildItem<IXCRoles>.Parent
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