using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
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
                var ser = new XmlSerializer(typeof(XCSingleEntityRule));
                var rule = ser.Deserialize(sr) as XCSingleEntityRule ?? throw new Exception();

                ((IChildItem<XCDataRuleContent>) rule).Parent = content;

                return rule;
            }
        }
    }

    public class SingleENtityConfigurationManager : ConfigurationManagerBase
    {
        private readonly XCComponent _component;

        public SingleENtityConfigurationManager(XCComponent component)
        {
            _component = component;
        }

        public override XCObjectTypeBase Create(XCObjectTypeBase baseType = null)
        {
            var newObj = new XCSingleEntity();
            ((IChildItem<XCComponent>) newObj).Parent = _component;
            newObj.BaseTypeId = baseType.Guid;

            _component.Parent.PlatformTypes.Add(newObj);

            return newObj;
        }

        public override void Delete(XCObjectTypeBase type)
        {
            _component.Parent.PlatformTypes.Remove(type);
        }
    }
}