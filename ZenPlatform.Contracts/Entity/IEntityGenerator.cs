using System.Collections.Generic;
using ZenPlatform.Contracts;

namespace ZenPlatform.DataComponent.Entity
{
  
}

namespace ZenPlatform.DataComponent.Entity
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