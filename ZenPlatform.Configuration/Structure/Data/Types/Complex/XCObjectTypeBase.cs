using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    /// <summary>
    /// Описание типа компонента
    /// Не забываем также реализовать свойсва, они загружаются в IComponentLoader
    /// </summary>
    public abstract class XCObjectTypeBase : XCTypeBase, IXCObjectType
    {
        private IXCComponent _parent;

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
        public IXCComponent Parent => _parent;

        /// <summary>
        /// Корень
        /// </summary>
        protected IXCRoot Root => _parent.Root;

        /// <summary>
        /// Раздел данных
        /// </summary>
        protected IXCData Data => Root.Data;

        /// <summary>
        /// Присоединённые файлы
        /// </summary>
        public IXCBlob AttachedBlob { get; set; }

        /// <summary>
        /// Родительский компонент
        /// </summary>
        IXCComponent IChildItem<IXCComponent>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        
        /// <summary>
        /// Имя связанной таблицы документа
        /// 
        /// При миграции присваивается движком. В последствии хранится в служебных структурах конкретной базы.
        /// </summary>
        //TODO: Продумать структуру, в которой будут храниться сопоставление Тип -> Дополнительные настройки компонента 
        /*
         * Результаты раздумий: Все мапинги должны быть в БД, а не в конфигурации. Оставляю TODO
         * выше просто для того, чтобы можно было поразмышлять,  вдруг я был не прав
         */
        [XmlIgnore]
        public string RelTableName { get; set; }

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
        /// Создать новое свойство
        /// </summary>
        /// <returns>Только что созданное свойство</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IXCObjectProperty CreateProperty()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить свойства объекта. Если объект не поддерживает свойства будет выдано NotSupportedException
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IXCObjectProperty> GetProperties()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Получить доступные программные модули объекта
        /// </summary>
        /// <returns>Список программных модулей</returns>
        /// <exception cref="NotSupportedException">Выдается в случае, если программные модули не поддерживаются компонентом</exception>
        public virtual IEnumerable<IXCProgramModule> GetProgramModules()
        {
            throw new NotSupportedException();
        }


        public virtual IXCObjectProperty GetPropertyByName(string name)
        {
            return GetProperties().First(x => x.Name == name);
        }

        /// <summary>
        /// Получить список доступных комманд у типа 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public virtual IEnumerable<IXCCommand> GetCommands()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Создать новую комманду
        /// </summary>
        /// <returns>Возвращается новая проинициализированная комманда</returns>
        /// <exception cref="NotSupportedException">Данная функция не поддерживается компонентом</exception>
        public virtual IXCCommand CreateCommand()
        {
            throw new NotSupportedException();
        }
    }
}