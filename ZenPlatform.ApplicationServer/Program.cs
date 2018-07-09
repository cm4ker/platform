using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization.Formatters;
using MessagePack;
using ZenPlatform.Configuration.ConfigurationLoader.Structure;
using ZenPlatform.Core;
using ZenPlatform.Core.Authentication;

namespace ZenPlatform.WorkProcess
{
    public static class Program
    {
        public static void Main(params string[] args)
        {
//            var configPath = args[1];
//
//            switch (args[0])
//            {
//                case "system": break;
//                case "work": break;
//            }
        }
    }

    /// <summary>
    /// Рабочий процесс. Обеспечивает связь между сервером и соответственно функциями платформы.
    /// Сервер содержит в себе настройки, которые передаёт рабочему процессу.
    /// Одновременно могут работать несколько рабочих процессов.
    /// Рабочий процесс может лишь манипулировать данными и иметь доступ к конфигурации только на чтение.
    /// </summary>
    public class WorkProcess
    {
        private PlatformEnvironment _env;

        public WorkProcess(string pathToConfiguration)
        {
            var config = XCRoot.Load(pathToConfiguration);
            _env = new PlatformEnvironment(config);
        }

        public void Start()
        {
            _env.Initialize();
        }

        public void Stop()
        {
            //TODO: Выгрузить все ресурсы, потребляемые процессом
        }


        /// <summary>
        /// Текущее состояние процесса
        /// </summary>
        public string Status { get; set; }


        /// <summary>
        /// Зарегистрировать соединение, т.е. создать сессию для соединения
        /// </summary>
        public void RegisterConnection(User user)
        {
            //TODO: выполнить проверку контрольной ссумы пользователя, для того, чтобы не получилось подмены
            _env.CreateSession(user);
        }
    }

    /// <summary>
    /// Системный процесс, позволяет обновлять структуру баз данных
    /// Также может обновлять структуру конфигурации
    /// </summary>
    public class SystemProcess
    {
        //TODO: добавить мигрирование
    }

    /*
     * Необходимо сделать несколько протоколов общения
     *
     * Типа всё - это микросервисы.
     *
     * 1) Сервер <-> Рабочий процесс
     * 2) Рабочий процесс <-> Сервер кэша и транзакций
     */


    public class WorkProcessProtocol
    {
        /*
         *Список команд:
         *     1) Получить объект (Ид, Маршрут)
         *     2) Получить список объектов (Маршрут)
         */
        
        public void ExecuteCommand()
        {
            
        }

        public void AuthorizeUser()
        {
        }
    }
}