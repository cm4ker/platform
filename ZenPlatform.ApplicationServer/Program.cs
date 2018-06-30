using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualBasic;
using ZenPlatform.Core;

namespace ZenPlatform.ApplicationServer
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
        private PlatformEnvironment _env;

        public WorkProcess()
        {
            _env = new PlatformEnvironment();
        }

        public void Start()
        {
            _env.Initialize();
        }

        public string Status { get; set; }


        /// <summary>
        /// Зарегистрировать соединение, т.е. создать сессию для соединения
        /// </summary>
        public void RegisterConnection()
        {
            var session = _env.CreateSession();
        }
    }

    /// <summary>
    /// Системный процесс, позволяет обновлять структуру баз данных
    /// Также может обновлять структуру конфигурации
    /// </summary>
    public class SystemProcess
    {
    }
}