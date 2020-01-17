using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Свойство сущности
    /// </summary>
    public class XCSingleEntityProperty : XCObjectPropertyBase, IChildItem<XCSingleEntity>,
        IEquatable<XCSingleEntityProperty>
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

        public bool Equals(XCSingleEntityProperty other)
        {
            if (other == null) return false;

            return (this.Guid == other.Guid) && (this.Name == other.Name) &&
                   this.Types.SequenceEqual(other.Types)
                   && this.Unique == other.Unique && this.IsSystemProperty == other.IsSystemProperty;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as XCSingleEntityProperty);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, Name, Unique, IsSystemProperty);
        }


        public override IEnumerable<XCColumnSchemaDefinition> GetPropertySchemas(string propName = null)
        {
            if (string.IsNullOrEmpty(propName)) propName = this.DatabaseColumnName;

            return PropertyHelper.GetPropertySchemas(propName, Types);
        }
    }

    public class XCSingleEntityLinkProperty : XCSingleEntityProperty
    {
        public override bool IsSelfLink => true;

        public override IEnumerable<XCColumnSchemaDefinition> GetPropertySchemas(string propName = null)
        {
            yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Ref, new XCGuid(), "Id");
            yield return new XCColumnSchemaDefinition(XCColumnSchemaType.NoSpecial, new XCString(), "Name");
        }
    }
}