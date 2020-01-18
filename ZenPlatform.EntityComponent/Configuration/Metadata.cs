using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class XCSingleEntityMetadataSettings : IXCSettingsItem
    {
        public List<XCSingleEntityProperty> Properties { get; set; }

        public List<XCSingleEntityModule> Modules { get; set; }

        public List<XCSingleEntityTable> Tables { get; set; }

        public List<XCCommand> Command { get; set; }

        public string Name { get; set; }

        public string TableName { get; set; }

        public Guid EntityId { get; set; }

        public Guid LinkId { get; set; }
    }

    public class XCSingleEntityMetadata : IXCTypeMetadata<XCSingleEntityMetadataSettings>
    {
        public List<XCSingleEntityProperty> Properties { get; }

        public List<XCSingleEntityModule> Modules { get; }

        public List<XCCommand> Command { get; }

        public List<XCSingleEntityTable> Tables { get; }

        public string Name { get; set; }

        public string TableName { get; set; }

        public Guid EntityId { get; set; }

        public Guid LinkId { get; set; }

        public XCSingleEntityMetadata()
        {
            Properties = new List<XCSingleEntityProperty>();

            Modules = new List<XCSingleEntityModule>();

            Command = new List<XCCommand>();
            
            Tables = new List<XCSingleEntityTable>();
        }


        public void Initialize(IXCLoader loader, XCSingleEntityMetadataSettings settings)
        {
            Properties.AddRange(settings.Properties);
            Modules.AddRange(settings.Modules);
            Command.AddRange(settings.Command);
            Tables.AddRange(settings.Tables);

            EntityId = settings.EntityId;
            LinkId = settings.LinkId;
            Name = settings.Name;
            TableName = settings.TableName;
        }

        public IXCSettingsItem Store(IXCSaver saver)
        {
            var settings = new XCSingleEntityMetadataSettings()
            {
                Modules = Modules,
                Properties = Properties,
                Command = Command,
                Name = Name,
                LinkId = LinkId,
                EntityId = EntityId,
                TableName = TableName
            };

            return settings;
        }
    }
}