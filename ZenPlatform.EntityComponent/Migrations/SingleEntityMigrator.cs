using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data.Entity;
using ZenPlatform.Configuration.Contracts.Migration;
using ZenPlatform.Configuration.Contracts.TypeSystem;
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

        private bool CheckDBChangesMainObject(IType x, IType y)
        {
            return !x.Properties.SequenceEqual(y.Properties);
        }

        private bool CheckDBChangesTable(ITable x, ITable y)
        {
            return !y.Properties.SequenceEqual(y.Properties);
        }

        private void ChangeDatabaseTable(IEntityMigrationScope scope, IType old, IType actual)
        {
            string tableName = $"{actual.GetSettings().DatabaseName}_tmp";

            //var task = new MigrationTaskAction(step, $"Change table {tableName}");

            AnalyzeProperties(scope, old.Properties, actual.Properties, tableName);
        }

        private void ChangeDatabaseTable(IEntityMigrationScope scope, ITable old,
            ITable actual)
        {
            string tableName = $"{actual.GetSettings().DatabaseName}_tmp";

            //var task = new MigrationTaskAction(step, $"Change table {tableName}");

            AnalyzeProperties(scope, old.Properties, actual.Properties, tableName);
        }

        private void AnalyzeProperties(IEntityMigrationScope scope, IEnumerable<IProperty> old,
            IEnumerable<IProperty> actual, string tableName)
        {
            var props = old.Where(p => !p.IsSelfLink)
                .FullJoin(
                    actual.Where(p => !p.IsSelfLink), x => x.Id,
                    x => new {old = x, actual = default(IProperty)},
                    x => new {old = default(IProperty), actual = x},
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

        public void MigrationPlan(IEntityMigrationPlan plan, IComponent oldState, IComponent actualState)
        {
            if (oldState == null && actualState != null)
            {
                var typesToCreate = actualState.GetTypes();

                typesToCreate.ForEach(e =>
                    plan.AddScope(
                        scope =>
                        {
                            scope.CreateTable(e.GetSettings().DatabaseName,
                                e.Properties.Where(p => !p.IsSelfLink).SelectMany(p => p.GetDbSchema()));

                            foreach (var table in e.Tables)
                            {
                                scope.CreateTable(table.GetSettings().DatabaseName,
                                    table.Properties.SelectMany(x => x.GetDbSchema()));
                            }
                        }, 20));
            }
            else if (oldState != null && actualState == null)
            {
                var typesToDelete = actualState.GetTypes();

                typesToDelete.ForEach(e =>
                    plan.AddScope(scope => scope.DeleteTable(e.GetSettings().DatabaseName), 20)
                );
            }
            else if (oldState != null && actualState != null)
            {
                var oldTypes = oldState.GetTypes();
                var actualTypes = actualState.GetTypes();

                var types = oldTypes.FullJoin(actualTypes, x => x.Id,
                    x => new {old = x, actual = default(IType)},
                    x => new {old = default(IType), actual = x},
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
                        plan.AddScope(scope => { scope.DeleteTable(old.GetSettings().DatabaseName); }, 30);
                    }
                    else if (old == null && actual != null)
                    {
                        plan.AddScope(
                            scope =>
                            {
                                scope.CreateTable(actual.GetSettings().DatabaseName,
                                    actual.Properties.Where(p => !p.IsSelfLink)
                                        .SelectMany(p => p.GetDbSchema()));

                                foreach (var table in actual.Tables)
                                {
                                    scope.CreateTable(table.GetSettings().DatabaseName,
                                        table.Properties.SelectMany(x => x.GetDbSchema()));
                                }
                            }, 20);
                    }
                    else
                    {
                        if (CheckDBChangesMainObject(old, actual))
                        {
                            var oldDbName = old.GetSettings().DatabaseName;
                            var actualDbName = actual.GetSettings().DatabaseName;

                            plan.AddScope(scope =>
                            {
                                scope.CopyTable(oldDbName, $"{actualDbName}_tmp");
                                scope.SetFlagCopyTable(oldDbName, $"{actualDbName}_tmp");
                            }, 10);

                            plan.AddScope(scope =>
                            {
                                ChangeDatabaseTable(scope, old, actual);
                                scope.SetFlagChangeTable(actualDbName);
                            }, 20);

                            plan.AddScope(scope =>
                            {
                                scope.DeleteTable(oldDbName);
                                scope.SetFlagDeleteTable(oldDbName);
                            }, 30);

                            plan.AddScope(scope =>
                            {
                                scope.RenameTable($"{actualDbName}_tmp", oldDbName);
                                scope.SetFlagRenameTable($"{actualDbName}_tmp");
                            }, 40);
                        }

                        //Не забываем. У нас ещё есть табличные части, которые лежат в отдельных таблицах
                        var oldTabes = old.Tables;
                        var actualTables = actual.Tables;

                        var tables = oldTabes.FullJoin(actualTables, x => x.Id,
                            x => new {old = x, actual = default(ITable)},
                            x => new {old = default(ITable), actual = x},
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

        private void MigrateTable(IEntityMigrationPlan plan, ITable old, ITable actual)
        {
            var actualDbName = actual.GetSettings().DatabaseName;
            var oldDbName = old.GetSettings().DatabaseName;

            plan.AddScope(scope =>
            {
                scope.CopyTable(oldDbName, $"{actualDbName}_tmp");
                scope.SetFlagCopyTable(oldDbName, $"{actualDbName}_tmp");
            }, 10);

            plan.AddScope(scope =>
            {
                ChangeDatabaseTable(scope, old, actual);
                scope.SetFlagChangeTable(actualDbName);
            }, 20);

            plan.AddScope(scope =>
            {
                scope.DeleteTable(oldDbName);
                scope.SetFlagDeleteTable(oldDbName);
            }, 30);

            plan.AddScope(scope =>
            {
                scope.RenameTable($"{actualDbName}_tmp", oldDbName);
                scope.SetFlagRenameTable($"{actualDbName}_tmp");
            }, 40);
        }

        public void CreateProperty(IEntityMigrationScope plan, IProperty property, string tableName)
        {
            property.GetDbSchema().ForEach(s => plan.AddColumn(s, tableName));
        }

        public void DeleteProperty(IEntityMigrationScope plan, IProperty property, string tableName)
        {
            property.GetDbSchema().ForEach(s => plan.DeleteColumn(s, tableName));
        }

        public void ChangeTable(IEntityMigrationScope plan, ITable old, ITable actual, string tableName)
        {
            AnalyzeProperties(plan, old.Properties, old.Properties, tableName);
        }

        public void ChangeProperty(IEntityMigrationScope plan, IProperty old, IProperty actual, string tableName)
        {
            //случай если в свойстве был один тип а стало много 
            if (old.Types.Count() == 1 && actual.Types.Count() > 1)
            {
                //ищем колонку для переименования
                var rename = old.GetDbSchema().Join(actual.GetDbSchema(),
                    o => o.PlatformType,
                    a => a.PlatformType,
                    (x, y) => new {old = x, actual = y}
                ).FirstOrDefault();

                //находим колонки которые нужно создать, это все кроме той что нужно переименовать
                var toCreate = actual.GetDbSchema()
                    .Where(s => s.FullName != rename.actual.FullName);
                // и добавляем их
                toCreate.ForEach(s => plan.AddColumn(s, tableName));

                // переименовываем колонку
                plan.RenameColumn(rename.old, rename.actual, tableName);

                //Обновляем колонку с типом
                plan.UpdateType(actual, tableName, rename.old.PlatformType);
            }
            else // было много стал один
            if (old.Types.Count() > 1 && actual.Types.Count() == 1)
            {
                //ищем колонку для переименования
                var rename = old.GetDbSchema().Join(actual.GetDbSchema(),
                    o => o.PlatformType,
                    a => a.PlatformType,
                    (x, y) => new {old = x, actual = y}
                ).FirstOrDefault();

                //находим колонки которые нужно удалить, это все кроме той что нужно переименовать
                var toDel = old.GetDbSchema().Where(s => s.FullName != rename.old.FullName);
                // и удаляем их
                toDel.ForEach(s => plan.DeleteColumn(s, tableName));

                // переименовываем колонку
                plan.RenameColumn(rename.old, rename.actual, tableName);
            }
            else
            {
                var schemas = old.GetDbSchema()
                    .FullJoin(
                        actual.GetDbSchema(),
                        x => x.FullName,
                        x => new {old = x, actual = default(ColumnSchemaDefinition)},
                        x => new {old = default(ColumnSchemaDefinition), actual = x},
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
                        if (schema.old.PlatformType.IsObject ||
                            schema.actual.PlatformType.IsObject)
                        {
                            if (schema.old.PlatformType.Id != schema.actual.PlatformType.Id)
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