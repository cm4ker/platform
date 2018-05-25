using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public class XmlConfRoles : IChildItem<XmlConfRoot>
    {
        private XmlConfRoot _parent;

        public XmlConfRoles()
        {
            Items = new ChildItemCollection<XmlConfRoles, XmlConfRole>(this);
        }

        [XmlArray("IncludedFiles")]
        [XmlArrayItem(ElementName = "File", Type = typeof(XmlConfFile))]
        public List<XmlConfFile> IncludedFiles { get; set; }

        public ChildItemCollection<XmlConfRoles, XmlConfRole> Items { get; }

        [XmlIgnore] public XmlConfRoot Parent => _parent;


        public void Load()
        {
            foreach (var includedFile in IncludedFiles)
            {
                var role = XmlConfHelper.DeserializeFromFile<XmlConfRole>(Path.Combine(XmlConfHelper.BaseDirectory,
                    includedFile.Path));

                Items.Add(role);
            }
        }

        XmlConfRoot IChildItem<XmlConfRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}