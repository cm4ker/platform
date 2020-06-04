using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    /// <summary>
    /// Компонент конфигурации
    /// </summary>
    public class Component
    {
        //private IXCComponentInformation _info;
        private object _componentImpl;


        private readonly IDictionary<CodeGenRuleType, CodeGenRule> _codeGenRules;
        private Assembly _componentAssembly;

        private TypeManager _tm;

        public Component(TypeManager tm)
        {
            _codeGenRules = new ConcurrentDictionary<CodeGenRuleType, CodeGenRule>();

            _tm = tm;
        }

        public Guid Id => Guid.Empty; //_info?.ComponentId ?? Guid.Empty;

        //
        public string Name => ""; // _info?.ComponentName;

        public TypeManager TypeManager => _tm;

        // /// <summary>
        // /// Информация о компоненте
        // /// </summary>
        // public IXCComponentInformation Info
        // {
        //     get => _info;
        //     set => _info = value;
        // }

        public Assembly ComponentAssembly
        {
            get => _componentAssembly;
            set { _componentAssembly = value; }
        }

        public object ComponentImpl
        {
            get => _componentImpl;
            set => _componentImpl = value;
        }


        /// <summary>
        /// Зарегистрировать правило для генерации кода
        /// Это действие иммутабельно. В последствии нельзя отменить регистрацию.
        /// Нельзя создавать два одинаковых правила генерации кода с одним типом. Это приведёт к ошибке
        /// </summary>
        /// <param name="rule"></param>
        public void RegisterCodeRule(CodeGenRule rule)
        {
            if (!_codeGenRules.ContainsKey(rule.Type))
                _codeGenRules.Add(rule.Type, rule);
            else
                throw new Exception("Нельзя регистрировать два правила с одинаковым типом");
        }

        /// <summary>
        ///  Получить правило генерации кода по его типу.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CodeGenRule GetCodeRule(CodeGenRuleType type)
        {
            return _codeGenRules[type];
        }

        public string GetCodeRuleExpression(CodeGenRuleType type)
        {
            return GetCodeRule(type).GetExpression();
        }
    }
}