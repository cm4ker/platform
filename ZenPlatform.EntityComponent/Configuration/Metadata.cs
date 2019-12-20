using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class XCSingleEntityMetadataSettings : IXCSettingsItem
    {
        public List<XCSingleEntityProperty> Properties { get; set; }

        public XCProgramModuleCollection<XCSingleEntity, XCSingleEntityModule> Modules { get; set; }

    }

    public class XCSingleEntityMetadata : IXCTypeMetadata<XCSingleEntityMetadataSettings>
    {
        

        public List<XCSingleEntityProperty> Properties { get; set; }

        public XCProgramModuleCollection<XCSingleEntity, XCSingleEntityModule> Modules { get; set; }

        public XCSingleEntityMetadata()
        {
            Properties = new List<XCSingleEntityProperty>();


        }

        public void AddPropertyRange(IEnumerable<XCSingleEntityProperty> properties)
        {
            Properties.AddRange(properties);
        }

        public void Initialize(IXCLoader loader, XCSingleEntityMetadataSettings settings)
        {
            Properties = settings.Properties;
            Modules = settings.Modules;
        }

        public IXCSettingsItem Store(IXCSaver saver)
        {
            var settings = new XCSingleEntityMetadataSettings()
            {
                Modules = Modules,
                Properties = Properties
            };

            return settings;
        }
    }
}