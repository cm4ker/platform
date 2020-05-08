using Aquila.Configuration.ConfigurationLoader.Contracts;
using Aquila.Configuration.Data;
using Aquila.DataComponent.Configuration;
using Aquila.DocumentComponent.Configuration;
using Aquila.DocumentComponent.Configuration.XmlConfiguration;

namespace Aquila.TableComponent.Configuration
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
