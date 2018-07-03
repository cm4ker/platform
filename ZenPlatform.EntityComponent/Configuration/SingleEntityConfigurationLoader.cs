using System;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.Structure;
using ZenPlatform.Configuration.ConfigurationLoader.Structure.Data;
using ZenPlatform.Contracts.Data;
using ZenPlatform.Contracts.ParenChildCollection;
using ZenPlatform.DataComponent.Configuration;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Этот клас автоматически будет использован в качестве загрузчика
    /// </summary>
    public class SingleEntityConfigurationLoader : ConfigurationLoaderBase<SingleEntity>
    {
        public override IDataComponent GetComponentImpl(XCComponent component)
        {
            return new DocumnetComponent(component);
        }

        protected override SingleEntity LoadObjectAction(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var ser = new XmlSerializer(typeof(SingleEntity));
                return ser.Deserialize(sr) as SingleEntity ?? throw new Exception();
            }
        }

        protected override XCDataRuleBase LoadRuleAction(XCDataRuleContent content)
        {
            using (var sr = new StringReader(content.RealContent))
            {
                var ser = new XmlSerializer(typeof(SingleEntityRule));
                var rule = ser.Deserialize(sr) as SingleEntityRule ?? throw new Exception();

                ((IChildItem<XCDataRuleContent>) rule).Parent = content;

                return rule;
            }
        }
    }
}