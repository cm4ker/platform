using System;
using System.Xml.Serialization;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.ConfigurationLoader.Structure.Data.Types.Complex
{
    /// <summary>
    /// Описание типа компонента
    /// Не забываем также реализовать свойсва, они загружаются в IComponentLoader
    /// </summary>
    public abstract class XCObjectTypeBase : XCTypeBase, IChildItem<XCComponent>
    {
        public XCObjectTypeBase()
        {
        }

        private XCComponent _parent;

        [XmlElement] public bool IsAbstract { get; set; }

        [XmlElement] public bool IsSealed { get; set; }

        [XmlAttribute] public Guid BaseTypeId { get; set; }

        [XmlIgnore] public XCComponent Parent => _parent;

        [XmlIgnore] protected XCRoot Root => _parent.Root;

        [XmlIgnore] protected XCData Data => Root.Data;

        XCComponent IChildItem<XCComponent>.Parent
        {
            get => _parent;
            set => _parent = value;
        }


        /// <summary>
        /// Инициализировать сущность.
        /// Для примера: здесь можно сделать регистрацию кэша объектов
        /// Вызывается после связки Компонент(Parent) -> Тип(Child)
        /// </summary>
        public virtual void Initialize()
        {
        }


        /// <summary>
        /// Загрузить зависимости.
        /// Внимание, этот метод вызывается после полной загрузки всех типов в конфигурации.
        /// Поэтому в нём можно обращаться к Data.PlatformTypes 
        /// </summary>
        public virtual void LoadDependencies()
        {
        }
    }
}