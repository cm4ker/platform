using System;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
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
            using (var sr = new StringReader(content.RealContent))
            {
                var ser = new XmlSerializer(typeof(DocumentRule));
                var rule = ser.Deserialize(sr) as DocumentRule ?? throw new Exception();

                ((IChildItem<XCDataRuleContent>) rule).Parent = content;

                return rule;
            }
        }
    }
}