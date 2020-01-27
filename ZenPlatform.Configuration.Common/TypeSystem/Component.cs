using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.TypeSystem
{
    /// <summary>
    /// Компонент конфигурации
    /// </summary>
    public class Component : IComponent
    {
        private bool _isLoaded;
        private XCComponentInformation _info;
        private IComponentLoader _loader;
        private IDataComponent _componentImpl;

        private List<MDType> _mdTypes;

        private readonly IDictionary<CodeGenRuleType, CodeGenRule> _codeGenRules;
        private Assembly _componentAssembly;

        private IXCRoot _parent;

        private ITypeManager _tm;

        public Guid Id => _info?.ComponentId ?? Guid.Empty;
        public string Name => _info?.ComponentName;

        public Component(ITypeManager tm)
        {
            _codeGenRules = new ConcurrentDictionary<CodeGenRuleType, CodeGenRule>();
            _mdTypes = new List<MDType>();
            _tm = tm;
        }

        /// <summary>
        /// Информация о компоненте
        /// </summary>
        public IXCComponentInformation Info
        {
            get => _info;
        }

        public bool IsLoaded
        {
            get => _isLoaded;
        }


        public Assembly ComponentAssembly
        {
            get => _componentAssembly;
            set
            {
                _componentAssembly = value;
                LoadComponentInformation();
            }
        }


        /// <summary>
        /// Загрузить инфомрацию о компоненте включая загрузчики, инфо, и так далее
        /// </summary>
        private void LoadComponentInformation()
        {
            var typeInfo = ComponentAssembly.GetTypes()
                .FirstOrDefault(x => x.BaseType == typeof(XCComponentInformation));

            if (typeInfo != null)
                _info = (XCComponentInformation) Activator.CreateInstance(typeInfo);
            else
                _info = new XCComponentInformation();

            var loaderType = ComponentAssembly.GetTypes()
                                 .FirstOrDefault(x =>
                                     x.IsPublic && !x.IsAbstract &&
                                     x.GetInterfaces().Contains(typeof(IComponentLoader))) ??
                             throw new Exception("Invalid component");

            _loader = (IComponentLoader) Activator.CreateInstance(loaderType);

            _componentImpl = _loader.GetComponentImpl(this);

            //Инициализируем компонент
            _componentImpl.OnInitializing();
        }

        public IXCRoot Parent => _parent;

        IXCRoot IChildItem<IXCRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public IComponentLoader Loader => _loader;

        public IDataComponent ComponentImpl => _componentImpl;


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

        public IMDComponent Metadata { get; set; }
        public ITypeManager TypeManager { get; }
    }
}