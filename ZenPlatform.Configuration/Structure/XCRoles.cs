using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{


    public class XCRoles : IXCRoles
    {
        private const string StandardRoleFolder = "Roles";

        private IXCRoot _parent;

        public XCRoles()
        {
            Items = new ChildItemCollection<IXCRoles, IXCRole>(this);
        }

        [XmlArray("Include")]
        [XmlArrayItem(ElementName = "Blob", Type = typeof(XCBlob))]
        public List<IXCBlob> Blobs { get; set; }

        public ChildItemCollection<IXCRoles, IXCRole> Items { get; }

        [XmlIgnore] public IXCRoot Parent => _parent;

        IXCRoot IChildItem<IXCRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        /// <summary>
        /// Сохранить роли
        /// </summary>
        public void Save()
        {
            /*
            foreach (var role in Items)
            {
                using (Stream stream = role.SerializeToStream())
                    Parent.Storage.SaveBlob(role.Name, StandardRoleFolder, stream);
            }
            */
        }

        /// <summary>
        /// Загрузить роли
        /// </summary>
        public void Load()
        {
            /*
            if (Blobs != null)
                foreach (var blob in Blobs)
                {
                    var stream = Parent.Storage.GetBlob(blob.Name, StandardRoleFolder);
                    var role = XCHelper.DeserializeFromStream<XCRole>(stream);

                    stream.Dispose();

                    Items.Add(role);

                    role.Load();
                }
                */
        }
    }
}