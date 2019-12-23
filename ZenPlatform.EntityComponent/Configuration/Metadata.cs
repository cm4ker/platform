using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class XCSingleEntityMetadataSettings : IXCSettingsItem
    {
        public List<XCSingleEntityProperty> Properties { get; set; }

        public List<XCSingleEntityModule> Modules { get; set; }

        public string Name { get; set; }

        public Guid EntityId { get; set; }

        public Guid LinkId { get; set; }

    }

    public class XCSingleEntityMetadata : IXCTypeMetadata<XCSingleEntityMetadataSettings>
    {
        

        public List<XCSingleEntityProperty> Properties { get; }

        public List<XCSingleEntityModule> Modules { get;  }

        public string Name { get; set; }

        public Guid EntityId { get; set; }

        public Guid LinkId { get; set; }

        public XCSingleEntityMetadata()
        {
            Properties = new List<XCSingleEntityProperty>();

            Modules = new List<XCSingleEntityModule>();
        }

        public void AddPropertyRange(IEnumerable<XCSingleEntityProperty> properties)
        {
            Properties.AddRange(properties);
        }

        public void AddModuleRange(IEnumerable<XCSingleEntityModule> modules)
        {
            Modules.AddRange(modules);
        }

        public void Initialize(IXCLoader loader, XCSingleEntityMetadataSettings settings)
        {
            AddPropertyRange(settings.Properties);
            AddModuleRange(settings.Modules);
            EntityId = settings.EntityId;
            LinkId = settings.LinkId;
            Name = settings.Name;
        }

        public IXCSettingsItem Store(IXCSaver saver)
        {
            var settings = new XCSingleEntityMetadataSettings()
            {
                Modules = Modules,
                Properties = Properties,
                Name = Name,
                LinkId = LinkId,
                EntityId = EntityId
            };

            return settings;
        }
    }
}