using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    public class XCRoles : IChildItem<XCRoot>
    {
        private const string StandardRoleFolder = "Roles";

        private XCRoot _parent;

        public XCRoles()
        {
            Items = new ChildItemCollection<XCRoles, XCRole>(this);
        }

        [XmlArray("Include")]
        [XmlArrayItem(ElementName = "Blob", Type = typeof(XCBlob))]
        public List<XCBlob> Blobs { get; set; }

        public ChildItemCollection<XCRoles, XCRole> Items { get; }

        [XmlIgnore] public XCRoot Parent => _parent;


        public void Load()
        {
            if (Blobs != null)
                foreach (var blob in Blobs)
                {
                    var stream = Parent.Storage.GetBlob(blob.Name, StandardRoleFolder);
                    var role = XCHelper.DeserializeFromStream<XCRole>(stream);

                    stream.Dispose();

                    Items.Add(role);

                    role.Load();
                }
        }

        XCRoot IChildItem<XCRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        /// <summary>
        /// Сохранить роли
        /// </summary>
        public void Save()
        {
            foreach (var role in Items)
            {
                using (Stream stream = role.SerializeToStream())
                    Parent.Storage.SaveBlob(role.Name, StandardRoleFolder, stream);
            }
        }
    }
}