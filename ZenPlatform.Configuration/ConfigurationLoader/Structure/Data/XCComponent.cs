using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;
using ZenPlatform.Contracts;
using ZenPlatform.Contracts.Data;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    /// <summary>
    /// Описание компонента
    /// </summary>
    public class XCComponent : IChildItem<XCData>
    {
        private XCData _parent;
        private bool _isLoaded;
        private ComponentInformation _info;
        private IXCLoader _loader;
        private IDataComponent _componentImpl;

        private readonly IDictionary<CodeGenRuleType, CodeGenRule> _codeGenRules;

        public XCComponent()
        {
            _codeGenRules = new ConcurrentDictionary<CodeGenRuleType, CodeGenRule>();
        }

        /// <summary>
        /// Информация о компоненте
        /// </summary>
        [XmlIgnore]
        public ComponentInformation Info
        {
            get => _info;
        }

        [XmlIgnore]
        public bool IsLoaded
        {
            get => _isLoaded;
        }

        [XmlElement] public XCFile File { get; set; }

        [XmlIgnore] public Assembly ComponentAssembly { get; set; }

        public void LoadComponent()
        {
            FileInfo fi = new FileInfo(Path.Combine(XCHelper.BaseDirectory, this.File.Path));

            if (!fi.Exists) throw new FileNotFoundException(fi.FullName);

            ComponentAssembly = Assembly.LoadFile(fi.FullName);

            var typeInfo = ComponentAssembly.GetTypes().FirstOrDefault(x => x.BaseType == typeof(ComponentInformation));

            if (typeInfo != null)
                _info = (ComponentInformation) Activator.CreateInstance(typeInfo);
            else
                _info = new ComponentInformation();

            var loaderType = ComponentAssembly.GetTypes()
                                 .FirstOrDefault(x =>
                                     x.IsPublic && !x.IsAbstract &&
                                     x.GetInterfaces().Contains(typeof(IXCLoader))) ??
                             throw new InvalidComponentException();


            _loader = (IXCLoader) Activator.CreateInstance(loaderType);

            _componentImpl = _loader.GetComponentImpl(this);
            _componentImpl.OnInitializing();
        }

        [XmlIgnore] public XCRoot Root => _parent.Parent;

        [XmlIgnore] public XCData Parent => _parent;

        [XmlIgnore] public IXCLoader Loader => _loader;

        [XmlIgnore] public IDataComponent ComponentImpl => _componentImpl;


        [XmlIgnore] public IEnumerable<XCObjectTypeBase> Types => _parent.ComponentTypes.Where(x => x.Parent == this);

        XCData IChildItem<XCData>.Parent
        {
            get => _parent;
            set => _parent = value;
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
    }
}