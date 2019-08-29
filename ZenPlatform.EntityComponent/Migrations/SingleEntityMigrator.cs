using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoreLinq.Extensions;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.DDL.CreateTable;
using ZenPlatform.QueryBuilder.DML.Update;
using ZenPlatform.Shared;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.EntityComponent.Migrations
{
    //TODO: Пока конфигурация не хранится в базе данных, делать какие-то телодвижения в миграциях бессмысленно, ибо нам неоткуда взять дифф

    public class SingleEntityMigrator : IEntityMigrator
    {
        private readonly IXCConfigurationSettingsStore _store;

        /// <summary>
        /// Создать новую миграцию для сущности
        /// </summary>
        /// <param name="store">хранилище настроек</param>
        /// <param name="old">Старая конфигурация</param>
        /// <param name="actual">Новая конфигурация</param>
        public SingleEntityMigrator(IXCConfigurationSettingsStore store)
        {
            _store = store;
        }

        private IList<SqlNode> GetScript(XCSingleEntity old, XCSingleEntity actual)
        {
            var result = new List<SqlNode>();

            if (old == null)
            {
                /*
                 * Создаём новую сущность
                 * Для этого необходимо проделать следующие действия:
                 *  1) Присвоить имена новым таблицам и колонкам
                 *  2) Сгенерировать скрипт создания таблиц
                 */

                //TODO: Проверить, присваиваются ли идентификаторы для XCTypeBase.Id автоматически. Если нет, то этим должна заниматься конфигурация
                actual.RelTableName = $"ent{actual.Id}";

                var createTable =
                    new CreateTableQueryNode(actual.RelTableName, (t) => { });

                foreach (var prop in actual.Properties)
                {
                    //Генерируем по следующему алгоритму
                    prop.DatabaseColumnName = $"fld{prop.Id}";

                    var hasObjectGuid = false;
                    var hasObjectInt = false;
                    foreach (var type in prop.Types)
                    {
                        if (type is XCPrimitiveType ptype)
                        {
                            ItemSwitch<XCPrimitiveType>.Switch(ptype)
                                .CaseIs<XCBoolean>(i =>
                                    createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}", x => x.Boolean()))
                                .CaseIs<XCString>(i => createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}",
                                    x => x.Varchar(i.ColumnSize)))
                                .CaseIs<XCDateTime>(i =>
                                    createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}", x => x.DateTime()))
                                .CaseIs<XCGuid>(i =>
                                    createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}", x => x.Guid()))
                                .CaseIs<XCNumeric>(i => createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}",
                                    x => x.Numeric(i.Scale, i.Precision)))
                                .CaseIs<XCBinary>(i => createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}",
                                    x => x.Varbinary(i.ColumnSize)));
                        }
                        else if (type is XCObjectTypeBase obj)
                        {
                            //TODO: по задумке дочерний компонент должен самостоятельно инкапсулировать поля для ссылки на него в чужие объекты
                            /*
                             * Опишим следующую ситуацию:
                             *
                             * У нас есть табличная часть. Она поставляется, как свойство в компонент.
                             * Но она не генерирует поле в основной таблице.
                             */

                            //Необхоидимо проверить, является ли тип принадлежащим к зависимому компоненту
                            if (obj.Parent.ComponentImpl.DatabaseObjectsGenerator.HasForeignColumn)
                            {
                                if (!hasObjectGuid)
                                {
                                    createTable.WithColumn($"{prop.DatabaseColumnName}_Ref", x => x.Guid());
                                    hasObjectGuid = true;
                                }
                                else if (!hasObjectInt)
                                {
                                    createTable.WithColumn($"{prop.DatabaseColumnName}_Type", x => x.Int());
                                    hasObjectInt = true;
                                }
                            }

                            //Если компонент приатаченный, в таком случае на свойство необходимо также запустить миграцию
                            if (actual.Parent.AttachedComponents.Contains(obj.Parent))
                            {
                                obj.Parent.ComponentImpl.Migrator.GetScript(null, obj);
                            }
                        }
                    }
                }

                result.Add(createTable);
            }

            if (actual == null)
            {
                result.Add(new DropTableQueryNode(old.RelTableName));
                // TODO: также необходимо удалить все дочерние комопненты
            }

            if (old != null && actual != null)
            {
                var props = old.Properties
                    .FullJoin(
                        actual.Properties, x => x.Guid, x => new {old = x, actual = default(XCSingleEntityProperty)},
                        x => new {old = default(XCSingleEntityProperty), actual = x},
                        (x, y) => new {old = x, actual = y});

                foreach (var prop in props)
                {
                    result.AddRange(GetPropertyScript(prop.old, prop.actual));
                }
            }

            return result;
        }


        public IList<SqlNode> GetPropertyScript(XCSingleEntityProperty old, XCSingleEntityProperty actual)
        {
            if (old == null && actual == null) throw new ArgumentNullException($"{nameof(old)} && {nameof(actual)}");

            var result = new List<SqlNode>();
            /*
             * Трансформации данных бывают разными:
             *
             * Например. Есть просто добавление или удаление колонки.
             * Это самое лёгкое.
             *
             * Самое сложно это процедура, когда необходимо изменить структуру одной колонки. Например.
             * У нас есть колонка с типом Int, мы также говорим, что там может храниться и строка (string), а потом ещё говорим, что
             * Туда можно запихать и объект и двоичные данные. А потом, после всего этого, мы удаляем типы данных и говорим что там теперь будет лежать дата
             *
             * Что делать со значениями? В случае если у нас сначлаа была строка, затем число, мы можем создать колонку, попробовать трансформировать данные
             * Если всё ок, то ништяк, если нет, выдавать ошибку и откатывать транзакцию.
             * 
             */

            if (actual == null)
            {
                // Свойство вовсе удалили. Необхоидмо удалить все колонки из базы данных

                var hasObjectGuid = false;
                var hasObjectInt = false;

                foreach (var type in old.Types)
                {
                    if (type is XCPrimitiveType ptype)
                    {
                        result.Add(new AlterTableQueryNode(old.Parent.RelTableName).DropColumn(
                            $"{old.DatabaseColumnName}_{type.Name}"));
                    }
                    else if (type is XCObjectTypeBase obj)
                    {
                        if (obj.Parent.ComponentImpl.DatabaseObjectsGenerator.HasForeignColumn)
                        {
                            if (!hasObjectGuid)
                            {
                                result.Add(new AlterTableQueryNode(old.Parent.RelTableName).DropColumn(
                                    $"{old.DatabaseColumnName}_Ref"));

                                hasObjectGuid = true;
                            }
                            else if (!hasObjectInt)
                            {
                                result.Add(new AlterTableQueryNode(old.Parent.RelTableName).DropColumn(
                                    $"{old.DatabaseColumnName}_Type"));

                                hasObjectInt = true;
                            }
                        }

                        //Если компонент приатаченный, в таком случае на свойство необходимо также запустить миграцию
                        if (old.Parent.Parent.AttachedComponents.Contains(obj.Parent))
                        {
                            result.AddRange(obj.Parent.ComponentImpl.Migrator.GetScript(obj, null));
                        }
                    }
                }
            }

            else if (old == null)
            {
                // Если свойство добавили, добавляем в таблицу колонки
                var hasObjectGuid = false;
                var hasObjectInt = false;
                foreach (var type in actual.Types)
                {
                    if (type is XCPrimitiveType ptype)
                    {
                        ItemSwitch<XCPrimitiveType>.Switch(ptype)
                            .CaseIs<XCBoolean>(i =>
                                result.Add(new AlterTableQueryNode(actual.Parent.RelTableName).AddColumn(
                                    $"{actual.DatabaseColumnName}_{type.Name}", x => x.Boolean())))
                            .CaseIs<XCString>(i =>
                                result.Add(new AlterTableQueryNode(actual.Parent.RelTableName).AddColumn(
                                    $"{actual.DatabaseColumnName}_{type.Name}", x => x.Varchar(i.ColumnSize))))
                            .CaseIs<XCDateTime>(i =>
                                result.Add(new AlterTableQueryNode(actual.Parent.RelTableName).AddColumn(
                                    $"{actual.DatabaseColumnName}_{type.Name}", x => x.DateTime())))
                            .CaseIs<XCGuid>(i =>
                                result.Add(new AlterTableQueryNode(actual.Parent.RelTableName).AddColumn(
                                    $"{actual.DatabaseColumnName}_{type.Name}", x => x.Guid())))
                            .CaseIs<XCNumeric>(i =>
                                result.Add(new AlterTableQueryNode(actual.Parent.RelTableName).AddColumn(
                                    $"{actual.DatabaseColumnName}_{type.Name}", x => x.Numeric(i.Scale, i.Precision))))
                            .CaseIs<XCBinary>(i =>
                                result.Add(new AlterTableQueryNode(actual.Parent.RelTableName).AddColumn(
                                    $"{actual.DatabaseColumnName}_{type.Name}", x => x.Varbinary(i.ColumnSize))));
                    }
                    else if (type is XCObjectTypeBase obj)
                    {
                        if (obj.Parent.ComponentImpl.DatabaseObjectsGenerator.HasForeignColumn)
                        {
                            if (!hasObjectGuid)
                            {
                                result.Add(new AlterTableQueryNode(actual.Parent.RelTableName).AddColumn(
                                    $"{actual.DatabaseColumnName}_Ref", x => x.Guid()));

                                hasObjectGuid = true;
                            }
                            else if (!hasObjectInt)
                            {
                                result.Add(new AlterTableQueryNode(actual.Parent.RelTableName).AddColumn(
                                    $"{actual.DatabaseColumnName}_Type", x => x.Int()));

                                hasObjectInt = true;
                            }
                        }

                        //Если компонент приатаченный, в таком случае на свойство необходимо также запустить миграцию
                        if (actual.Parent.Parent.AttachedComponents.Contains(obj.Parent))
                        {
                            result.AddRange(obj.Parent.ComponentImpl.Migrator.GetScript(null, obj));
                        }
                    }
                }
            }

            else
            {
                // Необхоимо промигрировать свойство
                var hasObjectGuid = false;
                var hasObjectInt = false;

                foreach (var oldType in old.Types)
                {
                    if (!actual.Types.Contains(oldType))
                    {
                        if (oldType is XCPrimitiveType ptype)
                        {
                            result.Add(new AlterTableQueryNode(old.Parent.RelTableName).DropColumn(
                                $"{old.DatabaseColumnName}_{oldType.Name}"));
                        }
                        else if (oldType is XCObjectTypeBase obj)
                        {
                            // Если в старом типе было больше одного объектного элемента и в новом тоже, то фактически, ничего делать не нужно
                            // Главное, чтобы у компонент, который поддерживает данный тип объекта был генератор свойств для чужих компонентов

                            // TODO: производить очистку данных после миграции

                            var oldTypesCount = old.Types.Count(x =>
                                x is XCObjectTypeBase tbase && tbase.Parent.ComponentImpl.DatabaseObjectsGenerator
                                    .HasForeignColumn);

                            var actualTypesCount = actual.Types.Count(x =>
                                x is XCObjectTypeBase tbase && tbase.Parent.ComponentImpl.DatabaseObjectsGenerator
                                    .HasForeignColumn);

                            if (oldTypesCount > 1 && actualTypesCount > 1)
                            {
                                // По структуре ничего не делаем, но вот по данным, я думаю что нужно подчищать Id, которые ушли
                            }
                            else if (oldTypesCount == 1 && actualTypesCount > 1)
                            {
                                //Если типов больше, чем один, нужно генерировать колонку Type и проставлять тип прошлой сущности
                                result.Add(new AlterTableQueryNode(old.Parent.RelTableName).AddColumn(
                                    $"{actual.DatabaseColumnName}_Type", x => x.Int()));

                                // Сразу же обновляем таблицу
                                result.Add(new UpdateQueryNode().Update(actual.Parent.RelTableName)
                                    .Set(f => f.Field($"{actual.DatabaseColumnName}_Type"),
                                        v => v.Raw(old.Id.ToString())));
                            }
                            else if (oldTypesCount > 1 && actualTypesCount == 1)
                            {
                                // Если мы оставляем один тип, то нет смысла держать дополнительную колонку. Удаляем её

                                result.Add(new AlterTableQueryNode(old.Parent.RelTableName).DropColumn(
                                    $"{old.DatabaseColumnName}_Type"));
                            }
                            else if (actualTypesCount == 0)
                            {
                                result.Add(new AlterTableQueryNode(old.Parent.RelTableName).DropColumn(
                                    $"{old.DatabaseColumnName}_Ref"));

                                if (oldTypesCount > 1)
                                    result.Add(new AlterTableQueryNode(old.Parent.RelTableName).DropColumn(
                                        $"{old.DatabaseColumnName}_Type"));
                            }

                            if (obj.Parent.ComponentImpl.DatabaseObjectsGenerator.HasForeignColumn)
                            {
                                if (!hasObjectGuid)
                                {
                                    result.Add(new AlterTableQueryNode(old.Parent.RelTableName).DropColumn(
                                        $"{old.DatabaseColumnName}_Ref"));

                                    hasObjectGuid = true;
                                }
                                else if (!hasObjectInt)
                                {
                                    result.Add(new AlterTableQueryNode(old.Parent.RelTableName).DropColumn(
                                        $"{old.DatabaseColumnName}_Type"));

                                    hasObjectInt = true;
                                }
                            }

                            //Если компонент приатаченный, в таком случае на свойство необходимо также запустить миграцию
                            if (old.Parent.Parent.AttachedComponents.Contains(obj.Parent))
                            {
                                result.AddRange(obj.Parent.ComponentImpl.Migrator.GetScript(obj, null));
                            }
                        }
                    }
                }
            }

            return result;
        }

        public IList<SqlNode> GetScript(XCObjectTypeBase old, XCObjectTypeBase actual)
        {
            return GetScript((XCSingleEntity) old, (XCSingleEntity) actual);
        }
    }
}