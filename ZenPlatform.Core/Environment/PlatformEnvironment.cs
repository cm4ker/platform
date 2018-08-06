using System.Collections.Generic;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Initializer;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Environment
{
    /// <summary>
    ///  Базовый класс среды, служит для того, чтобы описать две производные среды <see cref="WorkEnvironment"/> и <see cref="SystemEnvironment"/>
    /// </summary>
    public abstract class PlatformEnvironment
    {
        protected PlatformEnvironment(StartupConfig config)
        {
            Sessions = new List<ISession>();
            StartupConfig = config;

            SqlCompiler = SqlCompillerBase.FormEnum(config.DatabaseType);
        }

        protected SystemSession SystemSession { get; private set; }

        public virtual void Initialize()
        {
            SystemSession = new SystemSession(this, 1);

            //TODO: Дать возможность выбрать, какую конфигурацию загружать, с базы данных или из файловой системы

            var storage = new XCDatabaseStorage(DatabaseConstantNames.CONFIG_TABLE_NAME, SystemSession.GetDataContext(),
                SqlCompiler);

            Configuration = XCRoot.Load(storage);
        }

        /// <summary>
        /// Конфигурация базы данных
        /// </summary>
        public XCRoot Configuration { get; private set; }

        /// <summary>
        /// Сессии
        /// </summary>
        public IList<ISession> Sessions { get; }

        /// <summary>
        /// Стартовая конфигурация
        /// </summary>
        public StartupConfig StartupConfig { get; }


        /// <summary>
        /// Компилятор запросов, определяется на этапе инициализации приложения
        /// </summary>
        public SqlCompillerBase SqlCompiler { get; }
    }
}