﻿using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization.Formatters;
using MessagePack;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Sessions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.WorkProcess
{
    public static class Program
    {
        public static void Main(params string[] args)
        {
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
        private WorkEnvironment _env;

        public WorkProcess(StartupConfig config)
        {
            _env = new WorkEnvironment(config);
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
        //TODO: добавить мигрирование. Миграция конфигурации должно быть атомарным

        private SystemEnvironment _env;

        public SystemProcess(StartupConfig config)
        {
            _env = new SystemEnvironment(config);
        }

        public void Migrate()
        {
            /*
             * План мигрирования:
             *
             * 1) Удостовериться что у нас вторая конфигурация полностью рабочая (скомпилированна, с обновлённым блобом)
             * 2) Провести манипуляции с данными
             * 3) Подменить код сборки
             */
        }
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