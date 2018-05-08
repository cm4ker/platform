using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using ZenPlatform.Configuration.Data.Types.Complex;

namespace ZenPlatform.Configuration.Data
{
    /*
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

        public PComponent(Guid id)
        {
            _objects = new List<PObjectType>();
            _rules = new Dictionary<PGeneratedCodeRuleType, PGeneratedCodeRule>();
            Id = (id == Guid.Empty) ? Guid.NewGuid() : id;
        }

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
        /// Объекты, которые пренадлежат компоненту
        /// </summary>
        public IEnumerable<PObjectType> Objects
        {
            get { return _objects; }
        }

        /// <summary>
        /// Создать объект в компоненте
        /// </summary>
        /// <typeparam name="T">Тип объекта (унаследованный от PObjectType), который мы хотим создать</typeparam>
        /// <param name="name">Имя объекта</param>
        /// <returns></returns>
        [Obsolete(
            "Эта штука устарела, смотри комментарий в файле Configuration/Data/PComponent.cs. Кратко: теперь свойства создаются внутри компонента и затем должны обязательно быть зарегистрированны в PComponent")]
        public T CreateObject<T>(string name) where T : PObjectType
        {
            var obj = Activator.CreateInstance(typeof(T), name, Guid.NewGuid(), this) as T;
            _objects.Add(obj);
            return obj;
        }

        /// <summary>
        /// Регистрация объекта в компоненте
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterObject(PObjectType obj)
        {
            //Объект обязательно должен принадлежать этому компоненту, иначе регистрация не валидна
            if (obj.OwnerComponent != this)
                throw new InvalidOperationException(
                    "Нельзя регистрировать объект в компоненте, который ему не принадлежит");
            _objects.Add(obj);
        }

        public PObjectType GetObject(Guid id)
        {
            return _objects.FirstOrDefault(x => x.Id == id);
        }

        public PObjectType GetObject(string name)
        {
            return _objects.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Создать объект в компоненте
        /// </summary>
        /// <typeparam name="T">Тип объекта (унаследованный от PObjectType), который мы хотим создать</typeparam>
        /// <param name="name">Имя объекта</param>
        /// <param name="id">Идентификатор</param>
        /// <returns>Экземпляр объекта PObjectType</returns>
        public T CreateObject<T>(string name, Guid id) where T : PObjectType
        {
            var obj = Activator.CreateInstance(typeof(T), name, id, this) as T;
            _objects.Add(obj);
            return obj;
        }

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