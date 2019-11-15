using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
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
                   SequenceEqual<XCTypeBase>(this.Types, other.Types)
                   && this.Unique == other.Unique && this.IsSystemProperty == other.IsSystemProperty;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals(obj as XCSingleEntityProperty);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, Name, Unique, IsSystemProperty, Types);
        }


        private bool SequenceEqual<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }

            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }

            return cnt.Values.All(c => c == 0);
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
                Unique = true,
                IsReadOnly = true
            };
        }

        public static XCSingleEntityProperty CreateNameProperty()
        {
            return new XCSingleEntityProperty()
            {
                Name = "Name",
                Guid = Guid.Parse("905208c8-e892-414f-bd48-acd70b2a901b"),
                DatabaseColumnName = "Name",
                Types = {PlatformTypes.GetString(150)},
                IsSystemProperty = false,
                Unique = true,
                IsReadOnly = true
            };
        }
    }
}