using ZenPlatform.Configuration.Structure;
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
        /// Сохранённая конфигурация. При миграции, мы сравниваемся именно с ней
        /// </summary>
        public XCRoot SavedConfiguration { get; private set; }

        public void Migrate()
        {
            //if Configuration != SavedConfiguration


        }
    }
}