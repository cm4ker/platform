using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ZenPlatform.Core.Sessions;

namespace ZenPlatform.Core.Language
{
    /// <summary>
    /// Тип - запрос. Позволяет писать запросы абстрагируясь от субд.
    /// Работает исключительно с данными платформы
    /// </summary>
    public class Query
    {
        private readonly UserSession _session;

        /// <summary>
        /// Создать на основании пользовательской сессии
        /// </summary>
        /// <param name="session"></param>
        public Query(UserSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Создать на основании пользовательской сессии и текса запроса
        /// </summary>
        /// <param name="session"></param>
        /// <param name="text"></param>
        public Query(UserSession session, string text) : this(session)
        {
            Text = text;
        }

        /// <summary>
        /// Текст запроса 
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Выполнить запрос
        /// </summary>
        public void Execute()
        {
            //Чтобы корректно обработать запрос нам нужно следующее:
            // 1) Парсер
            // 2) Конфигурация
            // 

        }
    }
}
