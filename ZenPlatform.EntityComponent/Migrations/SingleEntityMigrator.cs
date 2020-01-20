using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data.Entity;
using ZenPlatform.Configuration.Contracts.Migration;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.EntityComponent.Configuration;

namespace ZenPlatform.EntityComponent.Migrations
{
    /*
     * Мигрирование происходит в несколько этапов
     * 1) Создание таблицы для нового типа структуры
     * 2) Перекачивание данных с трансформацией из старого типа структуры в новое
     * 3) Удаление старых структур
     * 4) Переименование новых структур
     */

    public class SingleEntityMigrator : IEntityMigrator
    {
        /// <summary>
        /// Создать новую миграцию для сущности
        /// </summary>
        /// <param name="store">хранилище настроек</param>
        /// <param name="old">Старая конфигурация</param>
        /// <param name="actual">Новая конфигурация</param>
        public SingleEntityMigrator()
        {
        }

        private bool CheckDBChangesMainObject(XCSingleEntity x, XCSingleEntity y)
        {
            return !x.Properties.Where(p => p is XCSingleEntityProperty)
                .SequenceEqual(y.Properties.Where(p => p is XCSingleEntityProperty));
        }

        private bool CheckDBChangesTable(XCSingleEntityTable x, XCSingleEntityTable y)
        {
            return !y.GetProperties().SequenceEqual(y.GetProperties());
        }

        private void ChangeDatabaseTable(IEntityMigrationScope scope, XCSingleEntity old, XCSingleEntity actual)
        {
            string tableName = $"{actual.RelTableName}_tmp";

            //var task = new MigrationTaskAction(step, $"Change table {tableName}");

            AnalyzeProperties(scope, old.Properties, actual.Properties, tableName);
        }

        private void ChangeDatabaseTable(IEntityMigrationScope scope, XCSingleEntityTable old,
            XCSingleEntityTable actual)
        {
            string tableName = $"{actual.RelTableName}_tmp";

            //var task = new MigrationTaskAction(step, $"Change table {tableName}");

            AnalyzeProperties(scope, old.Properties, actual.Properties, tableName);
        }

        private void AnalyzeProperties(IEntityMigrationScope scope, IEnumerable<IXCProperty> old,
            IEnumerable<IXCProperty> actual, string tableName)
        {
            var props = old.Where(p => !p.IsSelfLink)
                .FullJoin(
                    actual.Where(p => !p.IsSelfLink), x => x.Guid,
                    x => new {old = x, actual = default(IXCProperty)},
                    x => new {old = default(IXCProperty), actual = x},
                    (x, y) => new {old = x, actual = y});

            foreach (var property in props)
            {
                if (property.old == null)
                {
                    CreateProperty(scope, property.actual, tableName);
                }
                else if (property.actual == null)
                {
                    DeleteProperty(scope, property.old, tableName);
                }
                else
                {
                    if (!property.old.Equals(property.actual))
                        ChangeProperty(scope, property.old, property.actual, tableName);
                }
            }
        }

        public void MigrationPlan(IEntityMigrationPlan plan, IXCComponent oldState, IXCComponent actualState)
        {
            if (oldState == null && actualState != null)
            {
                var typesToCreate = actualState.Types.Where(t => t is XCSingleEntity).Cast<XCSingleEntity>();

                typesToCreate.ForEach(e =>
                    plan.AddScope(
                        scope =>
                        {
                            scope.CreateTable(e.RelTableName,
                                e.Properties.Where(p => !p.IsSelfLink).SelectMany(p => p.GetPropertySchemas()));

                            foreach (var table in e.Tables)
                            {
                                scope.CreateTable(table.RelTableName,
                                    table.GetProperties().SelectMany(x => x.GetPropertySchemas()));
                            }
                        }, 20));
            }
            else if (oldState != null && actualState == null)
            {
                var typesToDelete = actualState.Types.Where(t => t is XCSingleEntity).Cast<XCSingleEntity>();

                typesToDelete.ForEach(e =>
                    plan.AddScope(scope => scope.DeleteTable(e.RelTableName), 20)
                );
            }
            else if (oldState != null && actualState != null)
            {
                var oldTypes = oldState.Types.Where(t => t is XCSingleEntity).Cast<XCSingleEntity>();
                var actualTypes = actualState.Types.Where(t => t is XCSingleEntity).Cast<XCSingleEntity>();

                var types = oldTypes.FullJoin(actualTypes, x => x.Guid,
                    x => new {old = x, actual = default(XCSingleEntity)},
                    x => new {old = default(XCSingleEntity), actual = x},
                    (x, y) => new {old = x, actual = y});

                foreach (var entitys in types)
                {
                    var old = entitys.old;
                    var actual = entitys.actual;

                    if (old == null && actual == null)
                    {
                    }
                    else if (old != null && actual == null)
                    {
                        plan.AddScope(scope => { scope.DeleteTable(old.RelTableName); }, 30);
                    }
                    else if (old == null && actual != null)
                    {
                        plan.AddScope(
                            scope =>
                            {
                                scope.CreateTable(actual.RelTableName,
                                    actual.Properties.Where(p => !p.IsSelfLink)
                                        .SelectMany(p => p.GetPropertySchemas()));

                                foreach (var table in actual.Tables)
                                {
                                    scope.CreateTable(table.RelTableName,
                                        table.GetProperties().SelectMany(x => x.GetPropertySchemas()));
                                }
                            }, 20);
                    }
                    else
                    {
                        if (CheckDBChangesMainObject(old, actual))
                        {
                            plan.AddScope(scope =>
                            {
                                scope.CopyTable(old.RelTableName, $"{actual.RelTableName}_tmp");
                                scope.SetFlagCopyTable(old.RelTableName, $"{actual.RelTableName}_tmp");
                            }, 10);

                            plan.AddScope(scope =>
                            {
                                ChangeDatabaseTable(scope, old, actual);
                                scope.SetFlagChangeTable(actual.RelTableName);
                            }, 20);

                            plan.AddScope(scope =>
                            {
                                scope.DeleteTable(old.RelTableName);
                                scope.SetFlagDeleteTable(old.RelTableName);
                            }, 30);

                            plan.AddScope(scope =>
                            {
                                scope.RenameTable($"{actual.RelTableName}_tmp", old.RelTableName);
                                scope.SetFlagRenameTable($"{actual.RelTableName}_tmp");
                            }, 40);
                        }
                        
                        //Не забываем. У нас ещё есть табличные части, которые лежат в отдельных таблицах
                        var oldTabes = old.Tables.Cast<XCSingleEntityTable>();
                        var actualTables = actual.Tables.Cast<XCSingleEntityTable>();

                        var tables = oldTabes.FullJoin(actualTables, x => x.Guid,
                            x => new {old = x, actual = default(XCSingleEntityTable)},
                            x => new {old = default(XCSingleEntityTable), actual = x},
                            (x, y) => new {old = x, actual = y});

                        foreach (var t in tables)
                        {
                            if (CheckDBChangesTable(t.old, t.actual))
                            {
                                MigrateTable(plan, t.old, t.actual);
                            }
                        }
                    }
                }
            }
        }

        private void MigrateTable(IEntityMigrationPlan plan, XCSingleEntityTable old, XCSingleEntityTable actual)
        {
            plan.AddScope(scope =>
            {
                scope.CopyTable(old.RelTableName, $"{actual.RelTableName}_tmp");
                scope.SetFlagCopyTable(old.RelTableName, $"{actual.RelTableName}_tmp");
            }, 10);

            plan.AddScope(scope =>
            {
                ChangeDatabaseTable(scope, old, actual);
                scope.SetFlagChangeTable(actual.RelTableName);
            }, 20);

            plan.AddScope(scope =>
            {
                scope.DeleteTable(old.RelTableName);
                scope.SetFlagDeleteTable(old.RelTableName);
            }, 30);

            plan.AddScope(scope =>
            {
                scope.RenameTable($"{actual.RelTableName}_tmp", old.RelTableName);
                scope.SetFlagRenameTable($"{actual.RelTableName}_tmp");
            }, 40);
        }

        public void CreateProperty(IEntityMigrationScope plan, IXCProperty property, string tableName)
        {
            property.GetPropertySchemas().ForEach(s => plan.AddColumn(s, tableName));
        }

        public void DeleteProperty(IEntityMigrationScope plan, IXCProperty property, string tableName)
        {
            property.GetPropertySchemas().ForEach(s => plan.DeleteColumn(s, tableName));
        }

        public void ChangeTable(IEntityMigrationScope plan, IXCTable old, IXCTable actual, string tableName)
        {
            AnalyzeProperties(plan, old.GetProperties(), old.GetProperties(), tableName);
        }

        public void ChangeProperty(IEntityMigrationScope plan, IXCProperty old, IXCProperty actual, string tableName)
        {
            //случай если в свойстве был один тип а стало много 
            if (old.Types.Count == 1 && actual.Types.Count > 1)
            {
                //ищем колонку для переименования
                var rename = old.GetPropertySchemas().Join(actual.GetPropertySchemas(),
                    o => o.PlatformType,
                    a => a.PlatformType,
                    (x, y) => new {old = x, actual = y}
                ).FirstOrDefault();

                //находим колонки которые нужно создать, это все кроме той что нужно переименовать
                var toCreate = actual.GetPropertySchemas().Where(s => s.FullName != rename.actual.FullName);
                // и добавляем их
                toCreate.ForEach(s => plan.AddColumn(s, tableName));

                // переименовываем колонку
                plan.RenameColumn(rename.old, rename.actual, tableName);

                //Обновляем колонку с типом
                plan.UpdateType(actual, tableName, rename.old.PlatformType);
            }
            else // было много стал один
            if (old.Types.Count > 1 && actual.Types.Count == 1)
            {
                //ищем колонку для переименования
                var rename = old.GetPropertySchemas().Join(actual.GetPropertySchemas(),
                    o => o.PlatformType,
                    a => a.PlatformType,
                    (x, y) => new {old = x, actual = y}
                ).FirstOrDefault();

                //находим колонки которые нужно удалить, это все кроме той что нужно переименовать
                var toDel = old.GetPropertySchemas().Where(s => s.FullName != rename.old.FullName);
                // и удаляем их
                toDel.ForEach(s => plan.DeleteColumn(s, tableName));

                // переименовываем колонку
                plan.RenameColumn(rename.old, rename.actual, tableName);
            }
            else
            {
                var schemas = old.GetPropertySchemas()
                    .FullJoin(
                        actual.GetPropertySchemas(),
                        x => x.FullName,
                        x => new {old = x, actual = default(XCColumnSchemaDefinition)},
                        x => new {old = default(XCColumnSchemaDefinition), actual = x},
                        (x, y) => new {old = x, actual = y});

                foreach (var schema in schemas)
                {
                    if (schema.old == null)
                    {
                        plan.AddColumn(schema.actual, tableName);
                    }
                    else if (schema.actual == null)
                    {
                        plan.DeleteColumn(schema.old, tableName);
                    }
                    else
                    {
                        if (schema.old.PlatformType is XCObjectTypeBase ||
                            schema.actual.PlatformType is XCObjectTypeBase)
                        {
                            if (schema.old.PlatformType.Guid != schema.actual.PlatformType.Guid)
                                plan.AlterColumn(schema.actual, tableName);
                        }
                        else if (!schema.old.PlatformType.Equals(schema.actual.PlatformType))
                            plan.AlterColumn(schema.actual, tableName);
                    }
                }
            }
        }
    }
}