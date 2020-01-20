using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class MDSingleEntity : IMetaData<MDSingleEntitySettings>
    {
        public List<XCSingleEntityProperty> Properties { get; }

        public List<XCSingleEntityModule> Modules { get; }

        public List<XCCommand> Command { get; }

        public List<MDSingleEntityTable> Tables { get; }

        public string Name { get; set; }

        public string TableName { get; set; }

        public Guid EntityId { get; set; }

        public Guid LinkId { get; set; }

        public MDSingleEntity()
        {
            Properties = new List<XCSingleEntityProperty>();

            Modules = new List<XCSingleEntityModule>();

            Command = new List<XCCommand>();

            Tables = new List<MDSingleEntityTable>();
        }

        public void Initialize(IXCLoader loader, MDSingleEntitySettings settings)
        {
            Properties.AddRange(settings.Properties);
            Modules.AddRange(settings.Modules);
            Command.AddRange(settings.Command);

            EntityId = settings.EntityId;
            LinkId = settings.LinkId;
            Name = settings.Name;
            TableName = settings.TableName;

            foreach (var tabRef in settings.TableDefReferences)
            {
                Tables.Add(loader.LoadObject<MDSingleEntityTable, MDSingleEntityTableSettings>(tabRef));
            }
        }

        public IMDSettingsItem Store(IXCSaver saver)
        {
            var settings = new MDSingleEntitySettings()
            {
                Modules = Modules,
                Properties = Properties,
                Command = Command,
                Name = Name,
                LinkId = LinkId,
                EntityId = EntityId,
                TableName = TableName,
            };

            foreach (var table in Tables)
            {
                var tabRef = $"tbl_{table.Name}_{table.Guid}";
                settings.TableDefReferences.Add(tabRef);
                saver.SaveObject(tabRef, table);
            }

            return settings;
        }
    }
}