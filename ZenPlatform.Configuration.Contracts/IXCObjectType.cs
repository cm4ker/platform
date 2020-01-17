using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCStructureType : IXCType, IChildItem<IXCComponent>
    {
        /// <summary>
        /// Это ссылочный тип
        /// </summary>
        bool IsLink { get; }

        /// <summary>
        /// Это абстрактный тип
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Этот тип нельзя наследовать
        /// </summary>
        bool IsSealed { get; }

        /// <summary>
        /// Ссылка на базовый тип
        /// </summary>
        IXCType BaseType { get; }

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
        /// Объекта влияет на структуру базы данных
        /// </summary>
        bool HasDatabaseUsed { get; }

        /// <summary>
        /// Загрузить зависимости.
        /// Внимание, этот метод вызывается после полной загрузки всех типов в конфигурации.
        /// Поэтому в нём можно обращаться к Data.PlatformTypes 
        /// </summary>
        void LoadDependencies();

        /// <summary>
        /// Получить свойства объекта. Если объект не поддерживает свойства будет выдано NotSupportedException
        /// </summary>
        /// <returns></returns>
        IEnumerable<IXCProperty> GetProperties();

        /// <summary>
        /// Получить свойства объекта. Если объект не поддерживает свойства будет выдано NotSupportedException
        /// </summary>
        /// <returns></returns>
        IEnumerable<IXCTable> GetTables();


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
    }

    public interface IXCObjectType : IXCStructureType
    {
    }

    public static class StructHelper
    {
        public static IXCLinkType GetLink(this IXCObjectType type)
        {
            return (IXCLinkType) type.Parent.Types.FirstOrDefault(x =>
                x is IXCLinkType l && l.ParentType == type);
        }


        public static IXCProperty GetPropertyByName(this IXCStructureType type, string propName)
        {
            if (type.HasProperties)
                return type.GetProperties().FirstOrDefault(x => x.Name == propName) ??
                       throw new Exception($"Property not found: {propName}");
            else
                throw new Exception($"Component not support properties: {type.Parent.Info.ComponentName}");
        }
    }

    public interface IXCTable
    {
        /// <summary>
        /// Владелец таблицы
        /// </summary>
        IXCObjectType ParentType { get; }

        /// <summary>
        /// Название таблицы
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Получить свойства табличной части. Если объект не поддерживает свойства будет выдано NotSupportedException
        /// </summary>
        /// <returns></returns>
        IEnumerable<IXCProperty> GetProperties();
    }
}