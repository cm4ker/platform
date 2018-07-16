using System.ComponentModel.DataAnnotations.Schema;
using MoreLinq;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Configuration;

namespace ZenPlatform.Core.Environment
{
    /// <summary>
    /// Системная среда. Позволяет проводить изменения конфигурации, изменения структуры данных
    /// </summary>
    public class SystemEnvironment : PlatformEnvironment
    {
        public SystemEnvironment(StartupConfig config) : base(config)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            var storage = new XCDatabaseStorage("saved_conf", SystemSession.GetDataContext(), SqlCompiler);

            SavedConfiguration = XCRoot.Load(storage);
        }

        /// <summary>
        /// Сохранённая конфигурация.
        /// После того, как сохранённая конфигурация будет отличаться от конфигурации базы данных <see cref="PlatformEnvironment.Configuration"/> Выполняется применение.
        /// Причем при этом за кулисами происходит генерирование скрипта миграции
        /// </summary>
        public XCRoot SavedConfiguration { get; private set; }

        // Совершить миграцию базы данных
        public void Migrate()
        {
            var savedTypes = SavedConfiguration.Data.ComponentTypes;
            var dbTypes = Configuration.Data.ComponentTypes;

            var types = dbTypes.FullJoin(savedTypes, x => x.Guid,
                x => new {component = x.Parent, old = x, actual = default(XCObjectTypeBase)},
                x => new {component = x.Parent, old = default(XCObjectTypeBase), actual = x},
                (x, y) => new {component = x.Parent, old = x, actual = y});

            foreach (var type in types)
            {
                var migrateScript = type.component.ComponentImpl.Migrator.GetScript(type.old, type.actual);

                //TODO: Выполнить скрипт
            }
        }
    }
}