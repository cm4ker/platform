using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class MDSingleEntityTableSettings : IMDSettingsItem
    {
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public List<XCSingleEntityProperty> Properties { get; set; }
    }

    public class MDSingleEntityTable : IMetaData<MDSingleEntityTableSettings>
    {
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public List<XCSingleEntityProperty> Properties { get; set; }

        public void Initialize(IXCLoader loader, MDSingleEntityTableSettings settings)
        {
            Name = Name;

            Properties = settings.Properties;
        }

        public IMDSettingsItem Store(IXCSaver saver)
        {
            return new MDSingleEntityTableSettings
            {
                Properties = Properties,
                Name = Name
            };
        }
    }

    public class XCSingleEntityTable : XCTableBase, IChildItem<XCSingleEntity>, IEquatable<XCSingleEntityTable>
    {
        private readonly MDSingleEntityTable _metadata;
        private XCSingleEntity _parent;

        public XCSingleEntityTable(MDSingleEntityTable metadata)
        {
            _metadata = metadata;
        }

        public override Guid Guid
        {
            get => _metadata.Guid;
            set { }
        }

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

        public override string RelTableName => $"{_parent.RelTableName}_tb{Id}";

        public override IEnumerable<IXCProperty> GetProperties()
        {
            return _metadata.Properties;
        }
    }
}