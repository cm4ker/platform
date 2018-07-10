﻿using System;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Contracts.Data;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Этот клас автоматически будет использован в качестве загрузчика
    /// </summary>
    public class SingleEntityConfigurationLoader : ConfigurationLoaderBase<XCSingleEntity>
    {
        public override IDataComponent GetComponentImpl(XCComponent component)
        {
            return new EntityComponent(component);
        }

        protected override XCSingleEntity LoadObjectAction(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var ser = new XmlSerializer(typeof(XCSingleEntity));
                return ser.Deserialize(sr) as XCSingleEntity ?? throw new Exception();
            }
        }

        protected override XCDataRuleBase LoadRuleAction(XCDataRuleContent content)
        {
            using (var sr = new StringReader(content.RealContent))
            {
                var ser = new XmlSerializer(typeof(XCSingleEntityRule));
                var rule = ser.Deserialize(sr) as XCSingleEntityRule ?? throw new Exception();

                ((IChildItem<XCDataRuleContent>)rule).Parent = content;

                return rule;
            }
        }
    }
}