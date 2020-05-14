using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Aquila.Configuration.ConfigurationLoader.Contracts;
using Aquila.Configuration.Data;
using Aquila.Configuration.Exceptions;
using Aquila.DataComponent.Configuration;
using Aquila.ReferenceComponent.Configuration;
using Aquila.ReferenceComponent.Configuration.XmlConfiguration;

namespace Aquila.ReferenceComponent.Configuration
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
