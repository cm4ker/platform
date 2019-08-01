using System;
using System.Xml.Serialization;
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

        public XCSingleEntity Parent => _parent;

        XCSingleEntity IChildItem<XCSingleEntity>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public bool ShouldSerialize()
        {
            return !Unique;
        }

        public string GetFullPropertyName(XCTypeBase type)
        {
            if (Types.Count == 0) throw new InvalidOperationException();

            if (type is null) return Name;

            if (Types.Count == 1) return Name;

            if (type is)
        }
    }

    internal static class StandardEntityPropertyHelper
    {
        public static XCSingleEntityProperty CreateUniqueProperty()
        {
            return new XCSingleEntityProperty()
            {
                Name = "Id",
                Guid = Guid.Parse("905208c8-e892-414f-bd48-acd70b2a901b"),
                DatabaseColumnName = "Id",
                Types = {PlatformTypes.Guid},
                IsSystemProperty = true,
                Unique = true
            };
        }
    }
}