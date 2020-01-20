using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Common;
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

        public bool IsLink => false;

        /// <summary>
        /// Это абстрактный тип
        /// </summary>
        public virtual bool IsAbstract => true;

        public virtual bool IsSealed => throw new NotImplementedException();
        public virtual bool HasCommands => throw new NotImplementedException();

        public virtual bool HasProperties => throw new NotImplementedException();

        public virtual bool HasModules => throw new NotImplementedException();

        public virtual bool HasDatabaseUsed => throw new NotImplementedException();

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

        public virtual string RelTableName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IXCType BaseType { get; }

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


        IXCComponent IChildItem<IXCComponent>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        /// <summary>
        /// Получить свойства объекта. Если объект не поддерживает свойства будет выдано NotSupportedException
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IXCProperty> GetProperties()
        {
            throw new NotSupportedException();
        }

        public virtual IEnumerable<IXCTable> GetTables()
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


        public virtual IXCProperty GetPropertyByName(string name)
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
    }
}