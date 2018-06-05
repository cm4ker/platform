using System.Collections.Generic;


namespace ZenPlatform.Contracts.Entity
{
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

        CodeGenRule GetInForeignPropertySetActionRule();
        CodeGenRule GetInForeignPropertyGetActionRule();
        CodeGenRule GetNamespaceRule();

        /// <summary>
        /// Получить от компонента готовую сгенерированную структуру с разбивкой по файлам
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        Dictionary<string, string> GenerateFilesFromComponent();
    }
}