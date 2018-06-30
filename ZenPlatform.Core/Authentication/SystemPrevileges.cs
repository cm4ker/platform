namespace ZenPlatform.Core.Authentication
{
    /// <summary>
    /// Системные привелегии.
    /// Они обязательны, на уровне платформы
    /// </summary>
    public class SystemRight : RightBase
    {
        /// <summary>
        /// Пользователь являлется администратором данных 
        /// </summary>
        public bool IsDataAdministrator { get; set; }

        /// <summary>
        /// Пользователю разрешено обновлять базу данных
        /// </summary>
        public bool CanUpdateDataBase { get; set; }

        /// <summary>
        /// Монопольный режим управления данными
        /// </summary>
        public bool CanMonopolisticMode { get; set; }

        /// <summary>
        /// Пользователю разрешено подключаться к приложению (активный пользователь)
        /// </summary>
        public bool IsActiveUser { get; set; }

        /// <summary>
        /// Пользователю разрешено смотреть за логом регистрации данных
        /// </summary>
        public bool CanActionLogView { get; set; }

        /// <summary>
        /// Пользователю разрешено пользоваться тонким клиентом
        /// </summary>
        public bool CanUseThinClient { get; set; }

        /// <summary>
        /// Пользователю можно заходить через веб 
        /// </summary>
        public bool CanUseWebClient { get; set; }

        /// <summary>
        /// Пользователю разрешено подключаться через API для доступа к данным
        /// </summary>
        public bool CanUseExternalConnection { get; set; }

        /// <summary>
        /// Пользователю можно пользоваться внешними обработками    
        /// </summary>
        public bool CanOpenExternalModules { get; set; }

        /// <summary>
        /// Пользователю разрешено открывать консоль данных
        /// </summary>
        public bool ConsoleAccess { get; set; }
    }
}