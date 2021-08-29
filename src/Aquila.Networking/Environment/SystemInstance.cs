using System;
using Aquila.Core.CacheService;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Data;

namespace Aquila.Core.Environment
{
    /// <summary>
    /// Системная среда. Позволяет проводить изменения конфигурации, изменения структуры данных
    /// </summary>
    public class SystemInstance : PlatformInstance
    {
        public SystemInstance(DataContextManager dataContextManager, ICacheService cacheService) : base(
            dataContextManager, cacheService)
        {
        }

        public override void Initialize(IStartupConfig config)
        {
            base.Initialize(config);

            // var storage = new DatabaseFileSystem(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME,
            //     DataContextManager.GetContext());
            //
            // SavedConfiguration = _manipulator.Load(storage);
        }

        // /// <summary>
        // /// Сохранённая конфигурация.
        // /// После того, как сохранённая конфигурация будет отличаться от конфигурации базы данных <see cref="PlatformEnvironment.Configuration"/> Выполняется применение.
        // /// Причем при этом за кулисами происходит генерирование скрипта миграции
        // /// </summary>
        // public IProject SavedConfiguration { get; private set; }

        public override IInvokeService InvokeService => throw new NotImplementedException();

        public override IAuthenticationManager AuthenticationManager => throw new NotImplementedException();


        // Совершить миграцию базы данных
        public void Migrate()
        {
            /*
             * План мигрирования:
             *
             * 1) Удостовериться что у нас вторая конфигурация полностью рабочая (скомпилированна, с обновлённым блобом)
             * 2) Провести манипуляции с данными
             * 3) Подменить код сборки
             */

            var context = DataContextManager.GetContext();

            // var savedTypes = SavedConfiguration.TypeManager.StructureTypes;
            // var dbTypes = Configuration.Data.StructureTypes;

            /*
            var types = dbTypes.FullJoin(savedTypes, x => x.Guid,
                x => new {component = x.Parent, old = x, actual = default(IXCObjectType)},
                x => new {component = x.Parent, old = default(IXCObjectType), actual = x},
                (x, y) => new {component = x.Parent, old = x, actual = y});
                */
//            Expression query1 = new Expression();
//            Expression query2 = new Expression();
//            Expression query3 = new Expression();
//            Expression query4 = new Expression();

            //foreach (var type in types)
            // {
            /*
             * Все DDL (data definition language) происходят вне транзакции. Поэтому должны быть полностью
             * безопасны для внезапного прерывания.
             *
             * Необходимо логировать мигрирование каждого объекта 
             */
            //var migrateScript = type.component.ComponentImpl.Migrator.GetScript(type.old, type.actual);

//                query1.Nodes.Add(type.component.ComponentImpl.Migrator.GetStep1(type.old, type.actual).Expression);
//                query2.Nodes.Add(type.component.ComponentImpl.Migrator.GetStep2(type.old, type.actual).Expression);
//                query3.Nodes.Add(type.component.ComponentImpl.Migrator.GetStep3(type.old, type.actual).Expression);
//                query4.Nodes.Add(type.component.ComponentImpl.Migrator.GetStep4(type.old, type.actual).Expression);
            //}

//            var cmd = context.CreateCommand(query1);
//            cmd.ExecuteNonQuery();
//
//            cmd = context.CreateCommand(query2);
//            cmd.ExecuteNonQuery();
//
//            cmd = context.CreateCommand(query3);
//            cmd.ExecuteNonQuery();
//
//            cmd = context.CreateCommand(query4);
//            cmd.ExecuteNonQuery();


            //TODO: подменить код сборки и инвалидировать её, чтобы все участники обновили сборку.
        }

        public override ISession CreateSession(IUser user)
        {
            throw new NotImplementedException();
        }
    }
}