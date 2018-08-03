using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Contracts;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure.Data
{
    /// <summary>
    /// Компонент конфигурации
    /// </summary>
    public class XCComponent : IChildItem<XCData>
    {
        private XCData _parent;
        private bool _isLoaded;
        private XCComponentInformation _info;
        private IXComponentLoader _loader;
        private IDataComponent _componentImpl;

        private readonly IDictionary<CodeGenRuleType, CodeGenRule> _codeGenRules;

        public XCComponent()
        {
            _codeGenRules = new ConcurrentDictionary<CodeGenRuleType, CodeGenRule>();
            Include = new List<XCBlob>();
            AttachedComponentIds = new List<Guid>();
            AttachedComponents = new List<XCComponent>();
        }

        /// <summary>
        /// Информация о компоненте
        /// </summary>
        [XmlIgnore]
        public XCComponentInformation Info
        {
            get => _info;
        }

        [XmlIgnore]
        public bool IsLoaded
        {
            get => _isLoaded;
        }

        /// <summary>
        /// Хранилище компонента
        /// </summary>
        [XmlElement]
        public XCBlob Blob { get; set; }

        /// <summary>
        /// Список идентификаторов присоединённых компонентов
        /// </summary>
        [XmlArray("Attaches")]
        [XmlArrayItem(ElementName = "ComponentId", Type = typeof(Guid))]
        internal List<Guid> AttachedComponentIds { get; set; }

        /// <summary>
        /// Присоединённые компоненты. Это свойство инициализируется после загрузки всех компонентов
        /// </summary>
        [XmlIgnore]
        public List<XCComponent> AttachedComponents { get; private set; }

        /// <summary>
        /// Включенные файлы в компонент. Эти файлы будут загружены строго после загрузки компонента
        /// </summary>
        [XmlArray("Include")]
        [XmlArrayItem(ElementName = "Blob", Type = typeof(XCBlob))]
        public List<XCBlob> Include { get; set; }

        [XmlIgnore] public Assembly ComponentAssembly { get; set; }

        /// <summary>
        /// Загрузить все данные компонента из хранилища
        /// </summary>
        public void LoadComponent()
        {
            var blob = Root.Storage.GetBlob(Blob.Name, nameof(XCComponent));

            ComponentAssembly = Assembly.Load(blob);

            var typeInfo = ComponentAssembly.GetTypes()
                .FirstOrDefault(x => x.BaseType == typeof(XCComponentInformation));

            if (typeInfo != null)
                _info = (XCComponentInformation) Activator.CreateInstance(typeInfo);
            else
                _info = new XCComponentInformation();

            var loaderType = ComponentAssembly.GetTypes()
                                 .FirstOrDefault(x =>
                                     x.IsPublic && !x.IsAbstract &&
                                     x.GetInterfaces().Contains(typeof(IXComponentLoader))) ??
                             throw new InvalidComponentException();

            _loader = (IXComponentLoader) Activator.CreateInstance(loaderType);

            _componentImpl = _loader.GetComponentImpl(this);

            //Инициализируем компонент
            _componentImpl.OnInitializing();

            //Подгружаем все дочерние объекты
            foreach (var includeBlob in Include)
            {
                Loader.LoadObject(this, includeBlob);
            }

            _isLoaded = true;
        }

        /// <summary>
        /// Сохрнить все данные компонента в хранилище
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void SaveComponent()
        {
            //TODO: Реализовать механизм сохранения конфигурации в хранилище
            throw new NotImplementedException();
        }

        [XmlIgnore] public XCRoot Root => _parent.Parent;

        [XmlIgnore] public XCData Parent => _parent;

        [XmlIgnore] public IXComponentLoader Loader => _loader;

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