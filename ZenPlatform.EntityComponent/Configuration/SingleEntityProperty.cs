using System;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Свойство документа
    /// </summary>
    public class SingleEntityProperty : XCObjectPropertyBase, IChildItem<SingleEntity>
    {
        private SingleEntity _parent;

        public SingleEntityProperty()
        {
        }

        [XmlIgnore] public SingleEntity Parent => _parent;

        SingleEntity IChildItem<SingleEntity>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }

    internal static class StandartDocumentPropertyHelper
    {
        public static SingleEntityProperty CreatePostedProperty()
        {
            return new SingleEntityProperty()
            {
                Alias = "Posted",
                Id = Guid.Parse("27495aa2-4a50-4c3a-854c-564940aee515"),
                Types = {PlatformTypes.Boolean},
                IsSystemProperty = true,
                DatabaseColumnName = "IsPosted"
            };
        }

        public static SingleEntityProperty CreateUniqueProperty()
        {
            return new SingleEntityProperty()
            {
                Alias = "Id",
                Id = Guid.Parse("905208c8-e892-414f-bd48-acd70b2a901b"),
                Types = {PlatformTypes.Guid},
                IsSystemProperty = true,
                Unique = true
            };
        }
    }
}