using System;
using System.ComponentModel.DataAnnotations.Schema;
using MoreLinq;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Annotations;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.CacheService;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Data;
using ZenPlatform.Initializer;
using ZenPlatform.Core.DI;

namespace ZenPlatform.Core.Environment
{
    /// <summary>
    /// Системная среда. Позволяет проводить изменения конфигурации, изменения структуры данных
    /// </summary>
    public class SystemEnvironment : PlatformEnvironment
    {
        public SystemEnvironment(IDataContextManager dataContextManager, ICacheService cacheService) :
            base(dataContextManager, cacheService)
        {
        }

        public override void Initialize(StartupConfig config)
        {
            base.Initialize(config);

            var storage = new XCDatabaseStorage(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME,
                this.DataContextManager.GetContext(), DataContextManager.SqlCompiler);

            SavedConfiguration = XCRoot.Load(storage);
        }

        /// <summary>
        /// Сохранённая конфигурация.
        /// После того, как сохранённая конфигурация будет отличаться от конфигурации базы данных <see cref="PlatformEnvironment.Configuration"/> Выполняется применение.
        /// Причем при этом за кулисами происходит генерирование скрипта миграции
        /// </summary>
        public XCRoot SavedConfiguration { get; private set; }

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

            var savedTypes = SavedConfiguration.Data.ComponentTypes;
            var dbTypes = Configuration.Data.ComponentTypes;

            var types = dbTypes.FullJoin(savedTypes, x => x.Guid,
                x => new {component = x.Parent, old = x, actual = default(XCObjectTypeBase)},
                x => new {component = x.Parent, old = default(XCObjectTypeBase), actual = x},
                (x, y) => new {component = x.Parent, old = x, actual = y});

            foreach (var type in types)
            {
                /*
                 * Все DDL (data definition language) происходят вне транзакции. Поэтому должны быть полностью
                 * безопасны для внезапного прерывания.
                 *
                 * Необходимо логировать мигрирование каждого объекта 
                 */
                var migrateScript = type.component.ComponentImpl.Migrator.GetScript(type.old, type.actual);

                var cmd = context.CreateCommand();

                foreach (var node in migrateScript)
                {
                    cmd.CommandText = DataContextManager.SqlCompiler.Compile(node);
                    cmd.ExecuteNonQuery();
                }
            }

            //TODO: подменить код сборки и инвалидировать её, чтобы все участники обновили сборку.
        }

        public override ISession CreateSession(IUser user)
        {
            throw new NotImplementedException();
        }
    }
}