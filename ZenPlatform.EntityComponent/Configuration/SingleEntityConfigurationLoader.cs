using System;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Этот клас автоматически будет использован в качестве загрузчика
    /// </summary>
    public class SingleEntityConfigurationLoader : ConfigurationLoaderBase<XCSingleEntityMetadata, XCSingleEntityMetadataSettings>
    {
        public override IDataComponent GetComponentImpl(IXCComponent component)
        {
            return new EntityComponent(component);
        }

        public override void SaveObject(IXCObjectType conf, IXCSaver saver)
        {
            if (conf is XCSingleEntity entity)
            {
                saver.SaveObject(entity.Name, entity.GetMetadata());
            }
        }

        protected override void CreateType(XCSingleEntityMetadata metadata, IXCComponent component)
        {
            var entity = new XCSingleEntity(metadata);
            ((IChildItem<IXCComponent>)entity).Parent = component;

            component.RegisterType(entity);

            var link = new XCSingleEntityLink(entity, metadata);

            ((IChildItem<IXCComponent>)link).Parent = component;

            component.RegisterType(link);



        }

        protected override XCDataRuleBase LoadRuleAction(IXCDataRuleContent content)
        {
            using (var sr = new StringReader(content.RealContent))
            {
                var rule = XCHelper.Deserialize<XCSingleEntityRule>(sr.ReadToEnd()) ??
                           throw new Exception("Rule not loaded");

                ((IChildItem<IXCDataRuleContent>)rule).Parent = content;

                return rule;
            }
        }
    }
}