using System;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Свойство сущности
    /// </summary>
    public class XCSingleEntityProperty : XCObjectPropertyBase, IChildItem<XCSingleEntity>
    {
        private XCSingleEntity _parent;

        public XCSingleEntityProperty()
        {
        }

        [XmlIgnore] public XCSingleEntity Parent => _parent;

        XCSingleEntity IChildItem<XCSingleEntity>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }

    internal static class StandartDocumentPropertyHelper
    {
        public static XCSingleEntityProperty CreatePostedProperty()
        {
            return new XCSingleEntityProperty()
            {
                Alias = "Posted",
                Id = Guid.Parse("27495aa2-4a50-4c3a-854c-564940aee515"),
                Types = { PlatformTypes.Boolean },
                IsSystemProperty = true,
                DatabaseColumnName = "IsPosted"
            };
        }

        public static XCSingleEntityProperty CreateUniqueProperty()
        {
            return new XCSingleEntityProperty()
            {
                Alias = "Id",
                Id = Guid.Parse("905208c8-e892-414f-bd48-acd70b2a901b"),
                DatabaseColumnName = "Id",
                Types = { PlatformTypes.Guid },
                IsSystemProperty = true,
                Unique = true
            };
        }
    }
}