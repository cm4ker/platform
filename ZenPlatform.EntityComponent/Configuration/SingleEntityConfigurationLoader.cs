using System;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
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

        protected override XCDataRuleBase LoadRuleAction(XCDataRuleContent content)
        {
            using (var sr = new StringReader(content.RealContent))
            {
                var rule = XCHelper.Deserialize<XCSingleEntityRule>(sr.ReadToEnd()) ?? throw new Exception("Rule not loaded");

                ((IChildItem<XCDataRuleContent>) rule).Parent = content;

                return rule;
            }
        }
    }
}