using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Data.Types.Complex;

namespace ZenPlatform.Configuration.Data
{
    /*
     * UPDATE. Дискрэймлер: Всё фигня, Миша, давай по новой!
     *
     * Компонет НИКОГДА не выпячивает наружу создание PObjectType, а занимается этим внутри! Вообще, это не задача класса PComponent
     * Это лишь обёртка для взаимодействия с другими компоннетами. Все зарегистрированные объекты хранятся в конфигурации, в разделе Data
     *
     *  TODO: необходимо в классе PRootConfiguration, в разделе Data сделать коллекцию свойств и заполннять её при загрузке
     *
     * Всё что 
     *
     * Почему было избрано решение, что компонент конфигурации сам должен создавать объекты
     *
     * Объекты должны создаваться внутри Реализации компонента, например DocumentComponent
     *
     * Что нам нужно знать для того, чтобы успешно это сделать
     *     1)Мы должны корректно ссылаться на свойства, типы которых находятся вне данного компонента
     *     Например. У документа есть ссылка на справочную информацию. Мы должны через какой-то объект,
     *     предположительно PComponent получить доступ до реального компонента и выполнить его правила генерации
     *     кода и точно указать, как этот компонент объявляет колонки для чужих объектов и так далее
     *     2) Мы должны загружать объект из конфигурации и успешно выгружать его. Вся сложность в том, чтобы
     *     выгружать и загружать совйства, которые ещё не доступны по сути. Как реализовать загрузку, пока что не
     *     понятно. У нас есть свойство, тип которого находится в другом компоннете. Нам нужно его загрузить
     *     Сейчас конфигурация читается дважды, что, собственно интересно. Сначала прогружаются все объекты.
     *     без свойств, затем, прогружатся их свойства. На момент прогрузки у нас есть все необходимые типы.
     *     3) ОБЯЗАТЕЛЬНАЯ регистрация типа(PObjectType) в объекте PComponent
     */

    /// <summary>
    /// Класс компонента.
    /// 1) Обеспечивает интерфейс для создания новых объектов
    /// 2) Обеспечивает интерфейс для получения объектов
    /// 3) Компоненту также достутны интерфейсы для взаимодействием с генератором кода, чтобы сущности, которые генерируются
    /// были валидными и соответствовали определённому паттерну, фактически различным для каждого компонента.
    /// </summary>
    public class PComponent
    {
        private readonly IList<PObjectType> _objects;
        private readonly IDictionary<PGeneratedCodeRuleType, PGeneratedCodeRule> _rules;

        public PComponent(Guid id, IComponenConfigurationLoader loader)
        {
            _rules = new Dictionary<PGeneratedCodeRuleType, PGeneratedCodeRule>();
            Id = (id == Guid.Empty) ? Guid.NewGuid() : id;

            Loader = loader;
        }

        public PRootConfiguration Configuration { get; internal set; }

        /// <summary>
        /// Путь до библиотеки компонента
        /// </summary>
        public string ComponentPath { get; set; }

        /// <summary>
        /// Имя компонента
        /// (Документы, Справочники, Регистры и так далее)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор компонента
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Загрузчик конфигурации. Устанавливается при чтении корреного файла проекта
        /// </summary>
        public IComponenConfigurationLoader Loader { get; }

        /// <summary>
        /// Зарегистрировать правило для генерации кода
        /// Это действие иммутабельно. В последствии нельзя отменить регистрацию.
        /// Нельзя создавать два одинаковых правила генерации кода с одним типом. Это приведёт к ошибке
        /// </summary>
        /// <param name="rule"></param>
        public void RegisterCodeRule(PGeneratedCodeRule rule)
        {
            if (!_rules.ContainsKey(rule.Type))
                _rules.Add(rule.Type, rule);
            else
                throw new Exception("Нельзя регистрировать два правила с одинаковым типом");
        }

        /// <summary>
        ///  Получить правило генерации кода по его типу.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public PGeneratedCodeRule GetCodeRule(PGeneratedCodeRuleType type)
        {
            return _rules[type];
        }
    }
}