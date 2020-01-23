using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Shared.ParenChildCollection;
using static ZenPlatform.Configuration.Structure.XCData;

namespace ZenPlatform.Configuration.Structure
{
    public class XCDataConfig : IMDItem
    {
        public XCDataConfig()
        {
            ComponentReferences = new List<string>();
        }
        public List<string> ComponentReferences { get; set; }

    }
    public class XCData : IXCData, IMetaDataItem<XCDataConfig>
    {
        private IXCRoot _parent;
        private readonly ObservableCollection<IXCType> _platformTypes;
        private readonly ChildItemCollection<IXCData, IXCComponent> _components;

        

        public XCData()
        {
            _components = new ChildItemCollection<IXCData, IXCComponent>(this);
            _platformTypes = new ObservableCollection<IXCType>();

            //Инициализируем каждый тип, присваеваем ему идентификатор базы данных
            _platformTypes.CollectionChanged += (o, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var item = (e.NewItems[0] as TypeBase);
                    if (item != null)
                    {
                        uint id = item.Id;

                        _parent.Counter.GetId(item.Guid, ref id);

                        item.Id = id;
                    }
                }
            };
        }

        public ChildItemCollection<IXCData, IXCComponent> Components => _components;

        /// <summary>
        /// Загрузить дерективу и все зависимости
        /// </summary>
        public void Load()
        {
            //Инициализируем примитивные типы платформы, они нужны для правильного построения зависимостей
            /*
            _platformTypes.Add(new XCBinary());
            _platformTypes.Add(new XCString());
            _platformTypes.Add(new XCDateTime());
            _platformTypes.Add(new XCBoolean());
            _platformTypes.Add(new XCNumeric());
            _platformTypes.Add(new XCGuid());
            */
            //LoadComponents();

            foreach(var component in _components)
                foreach (var type in component.Types)
                    _platformTypes.Add(type);

            LoadDependencies();
        }

        #region Loading

        private void LoadComponents()
        {
            foreach (var component in Components.Where(x => !x.IsLoaded))
            {
                //component.LoadComponent();
            }
        }

        /// <summary>
        /// Загрузить файлы, относящиеся к разделу Data в дерективе IncludedFiles
        /// </summary>
        private void LoadDependencies()
        {
            //Загружаем присоединённые компоненты, чтобы можно было корректно генерировать зависимости
            foreach (XCComponent component in Components)
            {
                foreach (var attachedComponentId in component.AttachedComponentIds)
                {
                    component.AttachedComponents.Add(Components.First(x =>
                        x.Info.ComponentId == attachedComponentId && x.Info.AttachedComponent));
                }
            }

            //Загружаем зависимости типов
            foreach (var xct in StructureTypes)
            {
                xct.LoadDependencies();
            }
        }

        public void SetParent(XCRoot root)
        {
            _parent = root;
        }

        #endregion

        #region Saving

        /// <summary>
        /// Созрание всех данных из раздела Data
        /// </summary>
        public void Save()
        {
            foreach (var component in Components)
            {
               // component.SaveComponent();
            }
        }

        #endregion

        /// <summary>
        /// Все типы платформы
        /// </summary>
        public IEnumerable<IXCType> PlatformTypes => _platformTypes;

        /// <summary>
        /// Все типы, которые относятся к компонентам
        /// </summary>
        public IEnumerable<IXCStructureType> StructureTypes => _platformTypes.Where(s => s is IXCStructureType).Cast<IXCStructureType>();//Components.SelectMany(x => x.ObjectTypes);



        public IXCRoot Parent => _parent;


        IXCRoot IChildItem<IXCRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        /// <summary>
        /// Зарегистрировать тип данных на уровне конфигурации платформы
        /// </summary>
        /// <param name="type"></param>
        public void RegisterType(IXCType type)
        {
            _platformTypes.Add(type);
        }

        public IXCComponent GetComponentByName(string name)
        {
            return Components.FirstOrDefault(x => x.Info.ComponentName == name) ??
                   throw new Exception($"Component with name {name} not found");
        }

        public void Initialize(IXCLoader loader, XCDataConfig settings)
        {
            foreach (var reference in settings.ComponentReferences)
            {
                var component = loader.LoadObject<XCComponent, XCComponentConfig>(reference);
                ((IChildItem<IXCData>)component).Parent = this;
                
                _components.Add(component);
                
            }

            

        }

        public IMDItem Store(IXCSaver saver)
        {
            XCDataConfig settings = new XCDataConfig();
            foreach (var component in _components)
            {
                saver.SaveObject(component.Info.ComponentName, (XCComponent)component);
                settings.ComponentReferences.Add(component.Info.ComponentName);
            }

            return settings;
        }
    }
}