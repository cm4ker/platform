﻿using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.Configuration.Data.Contracts.Entity
{
    /// <summary>
    /// Генерация инструкций базы данных для других компоннетов 
    /// </summary>
    public interface IDatabaseObjectsGenerator
    {
        /*
         * Для того, чтобы сделать вытягивающий метод, необходимо реализовать этот интерфейс. Объясняю, про что это я.
         *
         * У нас есть тип, документ. Если мы сделаем новое свойство с типом "Документ", в таком случае мы должны в таблице, где создали свойство,
         * добавить колонку ссылки + тип на это совойство. (самое интересное ещё продумать для минимизации количество колонок)
         *
         * Теперь другая ситуация
         *
         * У нас есть компонент - ТабличнаяЧасть. Когда мы её добавляем как свойство, то замечаем следующее, в поле родителя свойства не добавляется ничего
         * А наоборот, добавляется колонка в табличную часть с сылкой на объект
         */

        //TODO: сделать генерацию умной, генерировать поле номера типа только тогда, когда оно действительно нужно (тип абстрактный и имеет зависимости)

        /// <summary>
        /// Получить опции для генерации колонки
        /// Key - Имя столбца
        /// Value - Тип столбца
        /// </summary>
        Dictionary<string, XCPremitiveType> GetColumnOptions();

        bool HasForeignColumn { get; }
    }
}