using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.Structure
{
    public class XCRoles : IChildItem<XCRoot>
    {
        private XCRoot _parent;

        public XCRoles()
        {
            Items = new ChildItemCollection<XCRoles, XCRole>(this);
        }

        [XmlArray("IncludedFiles")]
        [XmlArrayItem(ElementName = "File", Type = typeof(XCFile))]
        public List<XCFile> IncludedFiles { get; set; }

        public ChildItemCollection<XCRoles, XCRole> Items { get; }

        [XmlIgnore] public XCRoot Parent => _parent;


        public void Load()
        {
            foreach (var includedFile in IncludedFiles)
            {
                var role = XCHelper.DeserializeFromFile<XCRole>(Path.Combine(XCHelper.BaseDirectory,
                    includedFile.Path));
                Items.Add(role);

                role.Load();
            }
        }

        XCRoot IChildItem<XCRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}