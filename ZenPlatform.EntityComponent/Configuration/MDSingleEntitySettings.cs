using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class MDSingleEntitySettings : IMDSettingsItem
    {
        public List<XCSingleEntityProperty> Properties { get; set; }

        public List<XCSingleEntityModule> Modules { get; set; }

        public List<string> TableDefReferences { get; set; }

        public List<XCCommand> Command { get; set; }

        public string Name { get; set; }

        public string TableName { get; set; }

        public Guid EntityId { get; set; }

        public Guid LinkId { get; set; }

        public Guid ObjectId { get; set; }

        public Guid DtoId { get; set; }
    }


    public class MDProperty
    {
    }
}