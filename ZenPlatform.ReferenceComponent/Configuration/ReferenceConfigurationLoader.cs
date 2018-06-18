using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.ReferenceComponent.Configuration;
using ZenPlatform.ReferenceComponent.Configuration.XmlConfiguration;

namespace ZenPlatform.ReferenceComponent.Configuration
{
    public class ReferenceConfigurationLoader :
        ConfigurationLoaderBase<XmlConfReference, PReferenceObjectType, PReferenceObjectProperty>
    {

        protected override IComponentType CreateNewComponentType(PComponent component, XmlConfReference conf)
        {
            return new PReferenceObjectType(conf.Name, conf.Guid, component);
        }

        protected override void RelsolveDependencies(PReferenceObjectType obj, XmlConfReference conf, List<IComponentType> anotherDeps)
        {
            foreach (var property in conf.Properties)
            {
                var objProperty = new PReferenceObjectProperty(obj);
                objProperty.Alias = property.Alias;
                objProperty.DatabaseColumnName = property.DatabaseColumnName;

                if (string.IsNullOrEmpty(objProperty.DatabaseColumnName))
                    throw new ConfigurationPropertyColumnReferenceException();

                if (string.IsNullOrEmpty(objProperty.Alias))
                {
                    objProperty.Alias = objProperty.DatabaseColumnName;
                }

                obj.Properties.Add(objProperty);
            }

        }
    }
}
