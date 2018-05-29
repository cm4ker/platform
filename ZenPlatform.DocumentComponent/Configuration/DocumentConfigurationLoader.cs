﻿using System;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.DataComponent.Configuration;

namespace ZenPlatform.DocumentComponent.Configuration
{
    public class DocumentConfigurationLoader : ConfigurationLoaderBase<Document>
    {
        protected override Document LoadObjectAction(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var ser = new XmlSerializer(typeof(Document));
                return ser.Deserialize(sr) as Document ?? throw new Exception();
            }
        }

        protected override XCDataRuleBase LoadRuleAction(XCDataRuleContent content)
        {
            return new DocumentRule(content);
        }
    }
}