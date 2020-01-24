using System;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Этот клас автоматически будет использован в качестве загрузчика
    /// </summary>
    public class
        SingleEntityConfigurationLoader : ConfigurationLoaderBase<MDEntity, MDEntity>
    {
        public override void SaveTypeMD(IXCObjectType conf, IXCSaver saver)
        {
            if (conf is XCSingleEntity entity)
            {
                saver.SaveObject(entity.Name, entity.GetMetadata());
            }
        }

        protected override void CreateType(MDEntity metadata, IComponent component)
        {
            var entity = new XCSingleEntity(metadata);
            ((IChildItem<IComponent>) entity).Parent = component;

            component.RegisterType(entity);

            var link = new XCSingleEntityLink(entity, metadata);

            ((IChildItem<IComponent>) link).Parent = component;

            component.RegisterType(link);

            entity.Initialize();
        }

        protected override XCDataRuleBase LoadRuleAction(IXCDataRuleContent content)
        {
            using (var sr = new StringReader(content.RealContent))
            {
                var rule = XCHelper.Deserialize<XCSingleEntityRule>(sr.ReadToEnd()) ??
                           throw new Exception("Rule not loaded");

                ((IChildItem<IXCDataRuleContent>) rule).Parent = content;

                return rule;
            }
        }
    }
}