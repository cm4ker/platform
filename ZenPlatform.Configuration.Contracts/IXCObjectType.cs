using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{

    public interface IXCObjectReadOnlyType: IXCType, IChildItem<IXCComponent>
    {
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
        /// Загрузить зависимости.
        /// Внимание, этот метод вызывается после полной загрузки всех типов в конфигурации.
        /// Поэтому в нём можно обращаться к Data.PlatformTypes 
        /// </summary>
        void LoadDependencies();

        /// <summary>
        /// Получить свойства объекта. Если объект не поддерживает свойства будет выдано NotSupportedException
        /// </summary>
        /// <returns></returns>
        IEnumerable<IXProperty> GetProperties();

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

    public interface IXCObjectType : IXCObjectReadOnlyType
    {
    }

    public static class StructHelper
    {
        public static IXCLinkType GetLink(this IXCObjectType type)
        {
            return (IXCLinkType) type.Parent.Types.FirstOrDefault(x =>
                x is IXCLinkType l && l.ParentType == type);
        }


        public static IXProperty GetPropertyByName(this IXCObjectReadOnlyType type, string propName)
        {
            if (type.HasProperties)
                return type.GetProperties().FirstOrDefault(x => x.Name == propName) ??
                       throw new Exception($"Property not found: {propName}");
            else
                throw new Exception($"Component not support properties: {type.Parent.Info.ComponentName}");
        }
    }
}