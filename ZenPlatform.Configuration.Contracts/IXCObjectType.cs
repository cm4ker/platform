using System;
using System.Collections.Generic;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCObjectType : IXCType, IChildItem<IXCComponent>
    {
        /// <summary>
        /// Это абстрактный тип
        /// </summary>
        bool IsAbstract { get; set; }

        /// <summary>
        /// Этот тип нельзя наследовать
        /// </summary>
        bool IsSealed { get; set; }

        /// <summary>
        /// Ссылка на базовый тип
        /// </summary>
        Guid BaseTypeId { get; set; }

        /// <summary>
        /// Присоединённые файлы
        /// </summary>
        IXCBlob AttachedBlob { get; set; }

        /// <summary>
        /// У объекта есть поддержка свойств
        /// </summary>
        bool HasProperties { get; }


        string RelTableName { get; set; }
        
        /// <summary>
        /// Инициализировать сущность.
        /// Для примера: здесь можно сделать регистрацию кэша объектов
        /// Вызывается после связки Компонент(Parent) -> Тип(Child)
        /// </summary>
        void Initialize();

        /// <summary>
        /// Загрузить зависимости.
        /// Внимание, этот метод вызывается после полной загрузки всех типов в конфигурации.
        /// Поэтому в нём можно обращаться к Data.PlatformTypes 
        /// </summary>
        void LoadDependencies();

        /// <summary>
        /// Создать новое свойство
        /// </summary>
        /// <returns>Только что созданное свойство</returns>
        /// <exception cref="NotImplementedException"></exception>
        IXCObjectProperty CreateProperty();

        /// <summary>
        /// Получить свойства объекта. Если объект не поддерживает свойства будет выдано NotSupportedException
        /// </summary>
        /// <returns></returns>
        IEnumerable<IXCObjectProperty> GetProperties();

        /// <summary>
        /// Получить доступные программные модули объекта
        /// </summary>
        /// <returns>Список программных модулей</returns>
        /// <exception cref="NotSupportedException">Выдается в случае, если программные модули не поддерживаются компонентом</exception>
        IEnumerable<IXCProgramModule> GetProgramModules();

        IXCObjectProperty GetPropertyByName(string name);

        /// <summary>
        /// Получить список доступных комманд у типа 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        IEnumerable<IXCCommand> GetCommands();

        /// <summary>
        /// Создать новую комманду
        /// </summary>
        /// <returns>Возвращается новая проинициализированная комманда</returns>
        /// <exception cref="NotSupportedException">Данная функция не поддерживается компонентом</exception>
        IXCCommand CreateCommand();
    }


    public interface IXCProgramModule
    {
        /// <summary>
        /// Имя модуля
        /// </summary>
        string ModuleName { get; set; }

        /// <summary>
        /// Текст модуля
        /// </summary>
        string ModuleText { get; set; }

        /// <summary>
        /// Тип программного модуля
        /// </summary>
        XCProgramModuleDirectionType ModuleDirectionType { get; set; }

        XCProgramModuleRelationType ModuleRelationType { get; set; }
    }


    /// <summary>
    /// Тип модуля по отношению к объекту
    /// </summary>
    public enum XCProgramModuleRelationType
    {
        /// <summary>
        /// Модуль относится непосредственно к объекту
        /// </summary>
        Object,

        /// <summary>
        /// Модуль относится к менеджеру объектов
        /// </summary>
        Manager
    }

    /// <summary>
    /// Тип программного модуля
    /// </summary>
    public enum XCProgramModuleDirectionType
    {
        /// <summary>
        /// Серверный
        /// </summary>
        Server,

        /// <summary>
        /// Клиентский
        /// </summary>
        Client,

        /// <summary>
        /// Сервер-клиентский
        /// </summary>
        ClientServer
    }

    public interface IXCCommand
    {
        /// <summary>
        /// Уникальный идентификатор комманды
        /// </summary>
        Guid Guid { get; set; }

        /// <summary>
        /// Предпределенная ли это команда (не доступна для редактирования)
        /// </summary>
        bool IsPredefined { get; }

        /// <summary>
        /// Текстовое представление комманды
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Явное отображение комманды в интерфейсе
        /// </summary>
        string DisplayName { get; set; }

        IXCProgramModule Module { get; set; }

        /// <summary>
        /// Аргументы команды
        /// </summary>
        List<IXCDataExpression> Arguments { get; }
    }

    public interface IXCDataExpression
    {
    }
}