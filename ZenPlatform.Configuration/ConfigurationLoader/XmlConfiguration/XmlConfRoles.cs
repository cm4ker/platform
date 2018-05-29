using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public class XmlConfRoles : IChildItem<XCRoot>
    {
        private XCRoot _parent;

        public XmlConfRoles()
        {
            Items = new ChildItemCollection<XmlConfRoles, XCRole>(this);
        }

        [XmlArray("IncludedFiles")]
        [XmlArrayItem(ElementName = "File", Type = typeof(XmlConfFile))]
        public List<XmlConfFile> IncludedFiles { get; set; }

        public ChildItemCollection<XmlConfRoles, XCRole> Items { get; }

        [XmlIgnore] public XCRoot Parent => _parent;


        public void Load()
        {
            foreach (var includedFile in IncludedFiles)
            {
                var role = XmlConfHelper.DeserializeFromFile<XCRole>(Path.Combine(XmlConfHelper.BaseDirectory,
                    includedFile.Path));

                Items.Add(role);
            }
        }

        XCRoot IChildItem<XCRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}