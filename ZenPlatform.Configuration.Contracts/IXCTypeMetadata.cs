using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Метаданные типа
    /// </summary>
    public interface IXCTypeMetadata
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
        IXCType BaseType { get; set; }

        /// <summary>
        /// У объекта есть поддержка свойств
        /// </summary>
        bool HasProperties { get; }

        /// <summary>
        /// У объекта есть поддержка модулей
        /// </summary>
        bool HasModules { get; }

        /// <summary>
        /// У объекта есть поддержка комманд
        /// </summary>
        bool HasCommands { get; }

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
}