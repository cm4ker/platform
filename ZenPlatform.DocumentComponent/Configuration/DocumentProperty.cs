using System;
using System.Xml;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Primitive;
using ZenPlatform.DataComponent.Configuration;

namespace ZenPlatform.DocumentComponent.Configuration
{
    /// <summary>
    /// Свойство документа
    /// </summary>
    public class DocumentProperty : XCObjectPropertyBase, IChildItem<Document>
    {
        private Document _parent;

        public DocumentProperty()
        {
        }

        [XmlIgnore] public Document Parent => _parent;

        Document IChildItem<Document>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }

    internal static class StandartDocumentPropertyHelper
    {
        public static DocumentProperty CreatePostedProperty()
        {
            return new DocumentProperty()
            {
                Alias = "Posted",
                Id = Guid.Parse("27495aa2-4a50-4c3a-854c-564940aee515"),
                Types = { PlatformTypes.Binary },
                IsSystemProperty = true
            };
        }

        public static DocumentProperty CreateUniqueProperty()
        {
            return new DocumentProperty()
            {
                Alias = "Id",
                Id = Guid.Parse("905208c8-e892-414f-bd48-acd70b2a901b"),
                Types = { PlatformTypes.Guid },
                IsSystemProperty = true
            };
        }
    }
}