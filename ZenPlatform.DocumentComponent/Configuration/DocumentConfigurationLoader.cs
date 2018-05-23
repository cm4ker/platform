using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.DocumentComponent.Configuration.XmlConfiguration;

namespace ZenPlatform.DocumentComponent.Configuration
{
    public class DocumentConfigurationLoader : ConfigurationLoaderBase
        <XmlConfDocument, PDocumentObjectType, PDocumentObjectProperty>
    {
        protected override IComponentType CreateNewComponentType(PComponent component, XmlConfDocument conf)
        {
            return new PDocumentObjectType(conf.Name, conf.Id, component);
        }


        public override IRule LoadComponentRole(IComponentType obj, string xmlContent)
        {
            var docRule = xmlContent.Deserialize<XmlConfDocumentRule>();
            
            return new DefaultObjectRule(obj.Id, obj.OwnerComponent);
        }
    }

}
