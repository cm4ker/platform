using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
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

        /// <summary>
        /// Это абстрактный тип
        /// </summary>
        [XmlElement]
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Этот тип нельзя наследовать
        /// </summary>
        [XmlElement]
        public bool IsSealed { get; set; }

        /// <summary>
        /// Ссылка на базовый тип
        /// </summary>
        public Guid BaseTypeId { get; set; }

        /// <summary>
        /// Родительский компонент
        /// </summary>
        public XCComponent Parent => _parent;

        /// <summary>
        /// Корень
        /// </summary>
        protected XCRoot Root => _parent.Root;

        /// <summary>
        /// Раздел данных
        /// </summary>
        protected XCData Data => Root.Data;

        /// <summary>
        /// Присоединённые файлы
        /// </summary>
        public XCBlob AttachedBlob { get; set; }

        /// <summary>
        /// Родительский компонент
        /// </summary>
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

        /// <summary>
        /// У объекта есть поддержка свойств
        /// </summary>
        public virtual bool HasProperties { get; }

        /// <summary>
        /// Получить свойства объекта. Если объект не поддерживает свойства будет выдано NotSupportedException
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<XCObjectPropertyBase> GetProperties()
        {
            throw new NotSupportedException();
        }

        
        /// <summary>
        /// Получить доступные программные модули объекта
        /// </summary>
        /// <returns>Список программных модулей</returns>
        /// <exception cref="NotSupportedException">Выдается в случае, если программные модули не поддерживаются компонентом</exception>
        public virtual IEnumerable<XCProgramModuleBase> GetProgramModules()
        {
            throw new NotSupportedException();
        }


        public virtual XCObjectPropertyBase GetPropertyByName(string name)
        {
            return GetProperties().First(x => x.Name == name);
        }
    }
}