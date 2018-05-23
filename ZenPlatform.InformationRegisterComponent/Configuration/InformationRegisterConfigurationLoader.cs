using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.InformationRegisterComponent.Configuration.XmlConfiguration;

namespace ZenPlatform.InformationRegisterComponent.Configuration
{
    public class InformationRegisterConfigurationLoader : ConfigurationLoaderBase<
        XmlConfInformationRegister, PInformationRegisterObjectType, PInformationRegisterObjectProperty>
    {
        protected override IComponentType CreateNewComponentType(PComponent component, XmlConfInformationRegister conf)
        {
            return new PInformationRegisterObjectType(conf.Name, conf.Id, component);
        }
    }

}
