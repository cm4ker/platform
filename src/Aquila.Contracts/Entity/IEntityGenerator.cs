using System.Collections.Generic;


namespace Aquila.Contracts.Entity
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

    /// <summary>
    /// Генерация инструкций базы данных для других компоннетов 
    /// </summary>
    public interface IDatabaseObjectsGenerator
    {
        /*
         * Для того, чтобы сделать вытягивающий метод, необходимо реализовать этот интерфейс. Объясняю, про что это я.
         *
         * У нас есть тип, документ. Если мы сделаем новое свойство с типом "Документ", в таком случае мы должны в таблице, где создали свойство,
         * добавить колонку ссылки + тип на это совойство. (самое интересное ещё продумать для минимизации количество коло)
         *
         * Теперь другая ситуация
         *
         * У нас есть компонент - ТабличнаяЧасть. Когда мы её добавляем как свойство, то замечаем следующее, в поле родителя свойства не добавляется ничего
         * А наоборот, добавляется колонка в табличную часть с сылкой на объект
         */

        //TODO: сделать генерацию умной, генерировать поле номера типа только тогда, когда оно действительно нужно ( тип абстрактный и имеет зависимости)

        ///// <summary>
        ///// Получить опции для генерации колонки
        ///// </summary>
        //Dictionary<string, XCP> GetColumnOptions();
    }
}