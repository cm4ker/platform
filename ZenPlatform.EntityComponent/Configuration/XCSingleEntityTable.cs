using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class XCSingleEntityTable : XCTableBase, IChildItem<XCSingleEntity>, IEquatable<XCSingleEntityTable>
    {
        private XCSingleEntity _parent;

        /// <summary>
        /// Коллекция свойств табличной части
        /// </summary>
        public IEnumerable<IXCProperty> Properties => GetProperties();

        public XCSingleEntity Parent => _parent;

        XCSingleEntity IChildItem<XCSingleEntity>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public bool Equals(XCSingleEntityTable other)
        {
            if (other == null) return false;

            return (this.Guid == other.Guid) && (this.Name == other.Name) &&
                   this.Properties.SequenceEqual(other.Properties);
        }
    }
}