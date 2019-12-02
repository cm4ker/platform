using System;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <inheritdoc />
    public class SingleEntityConfigurationManager : ConfigurationManagerBase
    {
        public SingleEntityConfigurationManager(IXCComponent component) : base(component)
        {
        }

        public override IXCObjectType Create(IXCObjectType parentType = null)
        {
            var newItem = new XCSingleEntity();
            newItem.Guid = Guid.NewGuid();

            ((IChildItem<IXCComponent>) newItem).Parent = Component;
            Component.Parent.RegisterType(newItem);


            newItem.AttachedBlob = new XCBlob(Guid.NewGuid().ToString());
            Component.Include.Add(newItem.AttachedBlob);
            //TODO: Обработать базовый тип

            return newItem;
        }

        /// <inheritdoc />
        public override void Delete(IXCObjectType type)
        {
            //TODO: Сделать удаление сущности. Для этого необходимо сделать следующие задачи
            // 1) Проверка ссылочной целостности по объектам, т.е. узнать, нет ли ссылки на этот объект в других объектах
            // 2) Проверка целостности кода платформы, т.е. узнать, не используется ли этот компонент где-то в коде
            // Если одно  из  вышеперечисленных условий не выполнено, в таком случае нельзя давать удалять объект, а вывести список объектов, блокирующих удаление
        }
    }
}