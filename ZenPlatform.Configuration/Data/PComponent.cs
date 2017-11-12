using System;
using System.Collections.Generic;
using System.Text;

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
        /// Имя компонента
        /// (Документы, Справочники, Регистры и так далее)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Объекты, которые пренадлежат компоненту
        /// </summary>
        public IEnumerable<PObjectType> Objects
        {
            get { return _objects; }
        }

        public void RegisterObject(PObjectType obj)
        {
            _objects.Add(obj);
            obj.OwnerComponent = this;

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
