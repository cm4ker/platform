using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Shared.ParenChildCollection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Store;

namespace ZenPlatform.Configuration.Structure.Data
{
    public class XCComponentConfig : IMDSettingsItem
    {
        public string AssemblyReference { get; set; }
        
        public List<string> EntityReferences { get; set; }

        public XCComponentConfig()
        {
            EntityReferences = new List<string>();
        }
    }

    /// <summary>
    /// Компонент конфигурации
    /// </summary>
    public class XCComponent : IXCComponent, IMetaDataItem<XCComponentConfig>
    {
        private IXCData _parent;
        private bool _isLoaded;
        private XCComponentInformation _info;
        private IXComponentLoader _loader;
        private IDataComponent _componentImpl;

        private List<IXCType> _allTypes;

        private readonly IDictionary<CodeGenRuleType, CodeGenRule> _codeGenRules;
        private Assembly _componentAssembly;

        public XCComponent()
        {
            _codeGenRules = new ConcurrentDictionary<CodeGenRuleType, CodeGenRule>();
            _allTypes = new List<IXCType>();

            //Include = new XCBlobCollection();
            AttachedComponentIds = new List<Guid>();
            AttachedComponents = new List<IXCComponent>();
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


        /// <summary>
        /// Список идентификаторов присоединённых компонентов
        /// </summary>
        internal List<Guid> AttachedComponentIds { get; set; }

        /// <summary>
        /// Присоединённые компоненты. Это свойство инициализируется после загрузки всех компонентов
        /// </summary>
        public List<IXCComponent> AttachedComponents { get; private set; }

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
                                     x.GetInterfaces().Contains(typeof(IXComponentLoader))) ??
                             throw new InvalidComponentException();

            _loader = (IXComponentLoader) Activator.CreateInstance(loaderType);

            _componentImpl = _loader.GetComponentImpl(this);

            //Инициализируем компонент
            _componentImpl.OnInitializing();
        }

        public IXCRoot Root => _parent.Parent;

        public IXCData Parent => _parent;

        public IXComponentLoader Loader => _loader;


        public IDataComponent ComponentImpl => _componentImpl;

        public IEnumerable<IXCType> Types => _allTypes;

        public IEnumerable<IXCObjectType> ObjectTypes =>
            _allTypes.Where(x => x is IXCObjectType).Cast<IXCObjectType>();

        IXCData IChildItem<IXCData>.Parent
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

        public string GetCodeRuleExpression(CodeGenRuleType type)
        {
            return GetCodeRule(type).GetExpression();
        }


        public IXCObjectType GetTypeByName(string typeName)
        {
            return ObjectTypes.FirstOrDefault(x => x.Name == typeName) ??
                   throw new Exception($"Type with name {typeName} not found");
        }

        public void RegisterType(IXCType type)
        {
            _allTypes.Add(type);
            //Parent.RegisterType(type);
        }

        public void Initialize(IXCLoader loader, XCComponentConfig settings)
        {
            //load assembly
            var bytes = loader.LoadBytes(settings.AssemblyReference);

            var module = ModuleDefMD.Load(bytes);

            var alreadyLoaded = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.FullName == module.Assembly.FullName);

            if (alreadyLoaded != null)
                ComponentAssembly = alreadyLoaded;
            else
                ComponentAssembly = Assembly.Load(bytes);

            // load entitys
            foreach (var reference in settings.EntityReferences)
            {
                // XCSingleEntityMetadata loader.LoadObject<>(reference)
                Loader.LoadObject(this, loader, reference);
            }

            _isLoaded = true;
        }

        public IMDSettingsItem Store(IXCSaver saver)
        {
            XCComponentConfig settings = new XCComponentConfig();

            if (ComponentAssembly is null) return settings;

            foreach (var type in ObjectTypes)
            {
                Loader.SaveObject(type, saver);
                settings.EntityReferences.Add(type.Name);
            }

            var refelectionModule = ComponentAssembly.Modules.FirstOrDefault();
            ModuleDefMD module = ModuleDefMD.Load(refelectionModule);

            using (var ms = new MemoryStream())
            {
                module.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);

                saver.SaveBytes(refelectionModule.Name, ms.ToArray());

                settings.AssemblyReference = refelectionModule.Name;
            }

            return settings;
        }
    }
}