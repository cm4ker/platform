using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ZenPlatform.Configuration.Data
{
    public class PComponent
    {
        private readonly IList<PObjectType> _objects;
        private readonly IDictionary<PGeneratedCodeRuleType, PGeneratedCodeRule> _rules;
        public PComponent()
        {
            _objects = new List<PObjectType>();
            _rules = new Dictionary<PGeneratedCodeRuleType, PGeneratedCodeRule>();
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
        public T CreateObject<T>(string name) where T : PObjectType
        {
            var obj = Activator.CreateInstance(typeof(T), name, this) as T;
            _objects.Add(obj);
            return obj;
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

        public void RegisterCodeRule(PGeneratedCodeRule rule)
        {
            _rules.Add(rule.Type, rule);
        }

        public PGeneratedCodeRule GetCodeRule(PGeneratedCodeRuleType type)
        {
            return _rules[type];
        }
    }
}
