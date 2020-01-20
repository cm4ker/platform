using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.Data.Entity
{
    /// <summary>
    /// Отвечает за генерацию кода сущности
    /// </summary>
    public interface IEntityGenerator
    {
        /// <summary>
        /// Префикс объектов DTO, необходим для внутренних нужд класса
        /// </summary>
        string DtoPrefix { get; }

        string DtoPrivateFieldName { get; }

        /// <summary>
        /// Получить правило именования постфикса для сущности
        /// </summary>
        /// <returns></returns>
        CodeGenRule GetEntityClassPostfixRule();

        /// <summary>
        /// Получить правило именования префикса для сущности
        /// </summary>
        /// <returns></returns>
        CodeGenRule GetEntityClassPrefixRule();

        /// <summary>
        /// Получить имя класса dto
        /// </summary>
        /// <param name="obj">XCObjectTypeBase</param>
        /// <returns></returns>
        string GetDtoClassName(object obj);

        /// <summary>
        /// Получить имя класса сущности
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string GetEntityClassName(object obj);

        /// <summary>
        /// Поулчить имя класса, который оперирует мультитипом для определённого свойства
        /// </summary>
        /// <param name="property">Свойство для которого нужно сгенерировать мультикласс</param>
        /// <returns>Название класса</returns>
        string GetMultiDataStorageClassName(IXCProperty property);


        string GetMultiDataStoragePrivateFieldName(IXCProperty property);

        /// <summary>
        /// Получить код для setter'a в свойстве чужого объекта
        /// </summary>
        /// <returns>Правило генерации кода</returns>
        CodeGenRule GetInForeignPropertySetActionRule();

        /// <summary>
        /// Получить код для getter'a в свойстве чужого объекта
        /// </summary>
        /// <returns>Правило генерации кода</returns>
        CodeGenRule GetInForeignPropertyGetActionRule();

        /// <summary>
        /// Получить код для setter'a в свойстве чужого объекта
        /// </summary>
        /// <returns>Правило генерации кода</returns>
        CodeGenRule GetNamespaceRule();

        /// <summary>
        /// Получить от компонента готовую сгенерированную структуру с разбивкой по файлам
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GenerateSourceFiles();
    }
}