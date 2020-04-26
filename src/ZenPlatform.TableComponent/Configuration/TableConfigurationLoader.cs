using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Data;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.DocumentComponent.Configuration;
using ZenPlatform.DocumentComponent.Configuration.XmlConfiguration;

namespace ZenPlatform.TableComponent.Configuration
{
    public class TableConfigurationLoader : ConfigurationLoaderBase
        <XmlConfTable, PTableObjectType, PTableObjectProperty>
    {
        protected override IComponentType CreateNewComponentType(PComponent component, XmlConfTable conf)
        {
            return new PTableObjectType(conf.Name, conf.Guid, component);
        }
    }

}
