using System;
using System.IO;
using System.Xml.Serialization;
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

                ((IChildItem<XCDataRuleContent>)rule).Parent = content;

                return rule;
            }
        }
    }

    public class SingleEntityConfigurationManager : ConfigurationManagerBase
    {
        public SingleEntityConfigurationManager(XCComponent component) : base(component)
        {
        }

        public override XCObjectTypeBase Create(XCObjectTypeBase parentType = null)
        {
            var newItem = new XCSingleEntity();

            ((IChildItem<XCComponent>)newItem).Parent = Component;
            Component.Parent.RegisterType(newItem);
            
            //TODO: Обработать базовый тип
            
            return newItem;
        }

        /// <inheritdoc />
        public override void Delete(XCObjectTypeBase type)
        {
           //TODO: Сделать удаление компонента. Для этого необходимо сделать следующие задачи
           // 1) Проверка ссылочной целостности по объектам, т.е. узнать, нет ли ссылки на этот объект в других объектах
           // 2) Проверка целостности кода платформы, т.е. узнать, не используется ли этот компонент где-то в коде
           // Если одно  из  вышеперечисленных условий не выполнено, в таком случае нельзя давать удалять объект, а вывести список объектов, блокирующих удаление

        }
    }
}