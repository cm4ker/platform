using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using Antlr4.Runtime;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Core.Language.QueryLanguage;
using ZenPlatform.Core.Sessions;
using ZenPlatform.QueryBuilder.Common;

namespace ZenPlatform.Core.Language
{
    /// <summary>
    /// Тип - запрос. Позволяет писать запросы абстрагируясь от субд.
    /// Работает исключительно с данными платформы
    /// </summary>
    public class PlatformQuery
    {
        private readonly UserSession _session;
        private Dictionary<string, object> _parameters;

        /// <summary>
        /// Создать на основании пользовательской сессии
        /// </summary>
        /// <param name="session"></param>
        public PlatformQuery(UserSession session)
        {
            _session = session;
            _parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Создать на основании пользовательской сессии и текса запроса
        /// </summary>
        /// <param name="session"></param>
        /// <param name="text"></param>
        public PlatformQuery(UserSession session, string text) : this(session)
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
        public QueryDataReader Execute()
        {
            //Чтобы корректно обработать запрос нам нужно следующее:
            // 1) Парсер
            // 2) Конфигурация

            /*
             * FROM Document.Invoice I
             * WHERE i.Number LIKE '%test%'
             * GROUP BY i.Ref
             * HAVING COUNT()
             * GIVE i.Ref, COUNT() 
             */

            var sqlNode = Evaluate();
            var dataContext = ((PlatformEnvironment) _session.Environment).DataContextManager.GetContext();
            var command = dataContext.CreateCommand(sqlNode);

            foreach (var parameter in _parameters)
            {
                command.AddParameterWithValue(parameter.Key, parameter.Value);
            }

            /*
             *
             * UPDATE:
             * 1) Компонент действительно должен учавствовать в генерации запроса, но только для описания источника данных (для того, чтобы можно было делать виртуальные таблицы)
             * 2) Все взаимодействие полей разруливает сама платформа, компонент обязан предоставить наружу "хорошие" данные
             *
             *
             * 
             * Чтобы корректно обработать результат, нам нужно знать, что конкретно мы возвращяем. Пример:
             * У меня есть справочник, у него есть свойство составного типа ("SomethingProperty")
             *
             * Выглядит это вот так
             *
             * FROM Reference.MyRef r
             * SELECT r.SomethingProperty AS MyProperty
             *
             * тут наступает интересный момент.
             *
             * Мы должы пользователю вернуть ровно одну колонку
             *
             * Запрос у нас транслитируется в нечто вроде
             *
             * SELECT
             *  Fld20_String as MyProperty_String,
             *  Fld20_Date as MyProperty_Date,
             *  Fld20_Boolean as MyProperty_Boolean,
             *  Fld20_Guid as MyProperty_Guid,
             *  Fld20_Binary as MyProperty_Binary
             * FROM
             *  dbo.Ref3
             * 
             * Затем, эти множества колонок должны будут перейти в запрос
             *
             * FROM Reference.MyRef r
             *      JOIN Referemce.MyRef2 r2 ON r.SomethingProperty = r2.SomethingProperty
             * SELECT r.SomethingProperty AS MyProperty
             *
             * SELECT
             *  ... fields
             * FROM
             *  dbo.Ref3 exp1
             *  JOIN dbo.Ref3 exp2 ON %CASEEXP1% = %CASEEXP2%
             *
             * CASE WHEN Fld20_String = "Abc" OR Fld20_Boolean = "Abc" END
             */

            return new QueryDataReader(command.ExecuteReader());
        }

        public void SetParameter(string paramName, object paramValue)
        {
            if (_parameters.ContainsKey(paramName))
                _parameters[paramName] = paramName;
            else
                _parameters.Add(paramName, paramValue);
        }

        /// <summary>
        /// Обсчитать текст запроса. Здесь происходит превращение запроса из текста в объектную модель
        /// </summary>
        private SqlNode Evaluate()
        {
            var context = new DataQueryConstructorContext();
            context.Parameters = _parameters;

            AntlrInputStream inputStream = new AntlrInputStream(Text);
            ZSqlGrammarLexer speakLexer = new ZSqlGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            ZSqlGrammarParser speakParser = new ZSqlGrammarParser(commonTokenStream);
            ZSqlGrammarVisitor visitor =
                new ZSqlGrammarVisitor(((PlatformEnvironment) _session.Environment).Configuration, context);

            var result = visitor.Visit(speakParser.parse());

            return null;
        }
    }


    public class QueryDataReader
    {
        private readonly IDataReader _ddr;

        public QueryDataReader(IDataReader databaseDataReader)
        {
            _ddr = databaseDataReader;
        }
    }
}