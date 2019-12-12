﻿using System;
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

namespace ZenPlatform.Configuration.Structure.Data
{
    /// <summary>
    /// Компонент конфигурации
    /// </summary>
    public class XCComponent : IXCComponent
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

            Include = new XCBlobCollection();
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
        /// Хранилище компонента
        /// </summary>
        public IXCBlob Blob { get; set; }

        /// <summary>
        /// Список идентификаторов присоединённых компонентов
        /// </summary>
        internal List<Guid> AttachedComponentIds { get; set; }

        /// <summary>
        /// Присоединённые компоненты. Это свойство инициализируется после загрузки всех компонентов
        /// </summary>
        public List<IXCComponent> AttachedComponents { get; private set; }

        /// <summary>
        /// Включенные файлы в компонент. Эти файлы будут загружены строго после загрузки компонента
        /// </summary>
        public IXCBlobCollection Include { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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


        /// <summary>
        /// Загрузить все данные компонента из хранилища
        /// </summary>
        public void LoadComponent()
        {
            var stream = Root.Storage.GetBlob(Blob.Name, nameof(XCComponent));

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var bytes = ms.ToArray();
                var module = ModuleDefMD.Load(bytes);

                var alreadyLoaded = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(x => x.FullName == module.Assembly.FullName);

                if (alreadyLoaded != null)
                    ComponentAssembly = alreadyLoaded;
                else
                    ComponentAssembly = Assembly.Load(bytes);
            }


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
            if (ComponentAssembly is null) return;

            foreach (var type in ObjectTypes)
            {
                Loader.SaveObject(type);
            }

            if (Blob is null)
                Blob = new XCBlob(Path.GetFileName(ComponentAssembly.Location));

            var refelectionModule = ComponentAssembly.Modules.FirstOrDefault();
            ModuleDefMD module = ModuleDefMD.Load(refelectionModule);

            using (var ms = new MemoryStream())
            {
                module.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);

                Root.Storage.SaveBlob(Blob.Name, nameof(XCComponent), ms);
            }
        }

        public IXCRoot Root => _parent.Parent;

        public IXCData Parent => _parent;

        public IXComponentLoader Loader => _loader;


        public IDataComponent ComponentImpl => _componentImpl;

        public IEnumerable<IXCType> Types => _allTypes;

        public IEnumerable<IXCObjectType> ObjectTypes => _parent.ObjectTypes.Where(x => x.Parent == this);

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
        }
    }


    public class XCBlobCollection : List<IXCBlob>, IXCBlobCollection
    {
    }
}