using System.Collections.Generic;

namespace Aquila.Configuration.Contracts.Data.Entity
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