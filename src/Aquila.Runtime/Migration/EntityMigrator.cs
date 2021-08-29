using System.Collections.Generic;
using System.Linq;
using Aquila.Metadata;
using Aquila.Runtime;
using MoreLinq.Extensions;

namespace Aquila.Migrations
{
    /// <summary>
    /// Contains logic for migration. Need context for creating it
    ///
    /// For migrating we always have two states of metadata "old" and "new"
    /// </summary>
    public class EntityMigratorContext
    {
        /// <summary>
        /// Создать новую миграцию для сущности
        /// </summary>
        /// <param name="store">хранилище настроек</param>
        /// <param name="old">Старая конфигурация</param>
        /// <param name="actual">Новая конфигурация</param>
        public EntityMigratorContext()
        {
        }

        private bool CheckDBChangesMainObject(SMEntity x, SMEntity y)
        {
            return !x.Properties.SequenceEqual(y.Properties);
        }

        private string GetDbName(SMEntity md, EntityMigratorDataContext context)
        {
            var mdId = md.FullName;
            var descriptor = context.RuntimeContext.FindEntityDescriptor(mdId);

            if (descriptor == null)
            {
                descriptor = context.RuntimeContext.CreateDescriptor(context.ConnectionContext);
                descriptor.DatabaseName = $"Tbl_{descriptor.DatabaseId}";
                descriptor.MetadataId = mdId;
                context.RuntimeContext.Save(context.ConnectionContext);
            }

            return descriptor.DatabaseName;
        }

        // private bool CheckDBChangesTable(ITable x, ITable y)
        // {
        //     return !y.Properties.SequenceEqual(y.Properties);
        // }
        //
        private void ChangeDatabaseTable(EntityMigrationScope scope, SMEntity old, SMEntity actual,
            EntityMigratorDataContext context)
        {
            string tableName = $"{GetDbName(actual, context)}_tmp";

            //var task = new MigrationTaskAction(step, $"Change table {tableName}");

            AnalyzeProperties(scope, old.Properties, actual.Properties, tableName, context);
        }

        //
        // private void ChangeDatabaseTable(IEntityMigrationScope scope, ITable old,
        //     ITable actual)
        // {
        //     string tableName = $"{actual.GetSettings().DatabaseName}_tmp";
        //
        //     //var task = new MigrationTaskAction(step, $"Change table {tableName}");
        //
        //     AnalyzeProperties(scope, old.Properties, actual.Properties, tableName);
        // }
        //
        private void AnalyzeProperties(EntityMigrationScope scope, IEnumerable<SMProperty> old,
            IEnumerable<SMProperty> actual, string tableName, EntityMigratorDataContext context)
        {
            var props = old
                .FullJoin(
                    actual, x => x.Name,
                    x => new { old = x, actual = default(SMProperty) },
                    x => new { old = default(SMProperty), actual = x },
                    (x, y) => new { old = x, actual = y });

            foreach (var property in props)
            {
                if (property.old == null)
                {
                    CreateProperty(scope, property.actual, tableName, context);
                }
                else if (property.actual == null)
                {
                    DeleteProperty(scope, property.old, tableName, context.OldCollectionState, context);
                }
                else
                {
                    if (!property.old.Equals(property.actual))
                        ChangeProperty(scope, property.old, property.actual, tableName, context);
                }
            }
        }

        public void MigrationPlan(EntityMigrationPlan plan, EntityMigratorDataContext context)
        {
            var oldState = context.OldCollectionState.GetSemanticMetadata();
            var actualState = context.ActualCollectionState.GetSemanticMetadata();

            if (!oldState.Any() && actualState.Any())
            {
                var typesToCreate = context.ActualCollectionState.GetSemanticMetadata();

                typesToCreate.ForEach(e =>
                    plan.AddScope(
                        scope =>
                        {
                            scope.CreateTable(GetDbName(e, context),
                                e.Properties.SelectMany(p =>
                                    p.GetOrCreateSchema(context)));

                            //TODO: Add support for nested tables for Entity
                            // foreach (var table in e.Tables)
                            // {
                            //     scope.CreateTable(table.GetSettings().DatabaseName,
                            //         table.Properties.SelectMany(x => x.GetDbSchema()));
                            // }
                        }, 20));
            }
            else if (oldState.Any() && !actualState.Any())
            {
                var typesToDelete = oldState.ToList();

                typesToDelete.ForEach(e =>
                    plan.AddScope(scope => scope.DeleteTable(GetDbName(e, context)),
                        20)
                );
            }
            else if (oldState.Any() && !actualState.Any())
            {
                var oldTypes = oldState.ToList();
                var actualTypes = actualState.ToList();

                var types = oldTypes.FullJoin(actualTypes, x => x.Name,
                    x => new { old = x, actual = default(SMEntity) },
                    x => new { old = default(SMEntity), actual = x },
                    (x, y) => new { old = x, actual = y });

                foreach (var entitys in types)
                {
                    var old = entitys.old;
                    var actual = entitys.actual;

                    if (old == null && actual == null)
                    {
                    }
                    else if (old != null && actual == null)
                    {
                        plan.AddScope(
                            scope => { scope.DeleteTable(GetDbName(old, context)); }, 30);
                    }
                    else if (old == null && actual != null)
                    {
                        plan.AddScope(
                            scope =>
                            {
                                scope.CreateTable(GetDbName(actual, context),
                                    actual.Properties.SelectMany(p => p.GetOrCreateSchema(context)));


                                //TODO: Add support for nested tables for Entity
                                // foreach (var table in actual.Tables)
                                // {
                                //     scope.CreateTable(table.GetSettings().DatabaseName,
                                //         table.Properties.SelectMany(x => x.GetDbSchema()));
                                // }
                            }, 20);
                    }
                    else
                    {
                        if (CheckDBChangesMainObject(old, actual))
                        {
                            var oldDbName = GetDbName(old, context);
                            var actualDbName = GetDbName(actual, context);

                            plan.AddScope(scope =>
                            {
                                scope.CopyTable(oldDbName, $"{actualDbName}_tmp");
                                scope.SetFlagCopyTable(oldDbName, $"{actualDbName}_tmp");
                            }, 10);

                            plan.AddScope(scope =>
                            {
                                ChangeDatabaseTable(scope, old, actual, context);
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

                        // //Не забываем. У нас ещё есть табличные части, которые лежат в отдельных таблицах
                        // var oldTabes = old.Tables;
                        // var actualTables = actual.Tables;
                        //
                        // var tables = oldTabes.FullJoin(actualTables, x => x.Id,
                        //     x => new { old = x, actual = default(ITable) },
                        //     x => new { old = default(ITable), actual = x },
                        //     (x, y) => new { old = x, actual = y });
                        //
                        // foreach (var t in tables)
                        // {
                        //     if (CheckDBChangesTable(t.old, t.actual))
                        //     {
                        //         MigrateTable(plan, t.old, t.actual);
                        //     }
                        // }
                    }
                }
            }
        }

        //
        // private void MigrateTable(IEntityMigrationPlan plan, ITable old, ITable actual)
        // {
        //     var actualDbName = actual.GetSettings().DatabaseName;
        //     var oldDbName = old.GetSettings().DatabaseName;
        //
        //     plan.AddScope(scope =>
        //     {
        //         scope.CopyTable(oldDbName, $"{actualDbName}_tmp");
        //         scope.SetFlagCopyTable(oldDbName, $"{actualDbName}_tmp");
        //     }, 10);
        //
        //     plan.AddScope(scope =>
        //     {
        //         ChangeDatabaseTable(scope, old, actual);
        //         scope.SetFlagChangeTable(actualDbName);
        //     }, 20);
        //
        //     plan.AddScope(scope =>
        //     {
        //         scope.DeleteTable(oldDbName);
        //         scope.SetFlagDeleteTable(oldDbName);
        //     }, 30);
        //
        //     plan.AddScope(scope =>
        //     {
        //         scope.RenameTable($"{actualDbName}_tmp", oldDbName);
        //         scope.SetFlagRenameTable($"{actualDbName}_tmp");
        //     }, 40);
        // }
        //
        public void CreateProperty(EntityMigrationScope plan, SMProperty property, string tableName,
            EntityMigratorDataContext context)
        {
            foreach (var col in property.GetOrCreateSchema(context))
            {
                plan.AddColumn(col, tableName);
            }
        }

        public void DeleteProperty(EntityMigrationScope plan, SMProperty property, string tableName,
            EntityMetadataCollection mdCol, EntityMigratorDataContext context)
        {
            property.GetOrCreateSchema(context).ForEach(s => plan.DeleteColumn(s, tableName));
        }

        // public void ChangeTable(IEntityMigrationScope plan, ITable old, ITable actual, string tableName)
        // {
        //     AnalyzeProperties(plan, old.Properties, old.Properties, tableName);
        // }
        //
        public void ChangeProperty(EntityMigrationScope plan, SMProperty old, SMProperty actual,
            string tableName, EntityMigratorDataContext context)
        {
            var oldMdCol = context.OldCollectionState;
            var actualMdCol = context.ActualCollectionState;

            //случай если в свойстве был один тип а стало много 
            if (old.Types.Count() == 1 && actual.Types.Count() > 1)
            {
                //ищем колонку для переименования
                var rename = old.GetOrCreateSchema(context).Join(
                    actual.GetOrCreateSchema(context),
                    o => o.Type,
                    a => a.Type,
                    (x, y) => new { old = x, actual = y }
                ).FirstOrDefault();

                //находим колонки которые нужно создать, это все кроме той что нужно переименовать
                var toCreate = actual.GetOrCreateSchema(context)
                    .Where(s => s.FullName != rename.actual.FullName);
                // и добавляем их
                toCreate.ForEach(s => plan.AddColumn(s, tableName));

                // переименовываем колонку
                plan.RenameColumn(rename.old, rename.actual, tableName);

                //Обновляем колонку с типом
                plan.UpdateType(actual, tableName, rename.old.Type);
            }
            else // было много стал один
            if (old.Types.Count() > 1 && actual.Types.Count() == 1)
            {
                //ищем колонку для переименования
                var rename = old.GetOrCreateSchema(context).Join(
                    actual.GetOrCreateSchema(context),
                    o => o.Type,
                    a => a.Type,
                    (x, y) => new { old = x, actual = y }
                ).FirstOrDefault();

                //находим колонки которые нужно удалить, это все кроме той что нужно переименовать
                var toDel = old.GetOrCreateSchema(context).Where(s => s.FullName != rename.old.FullName);
                // и удаляем их
                toDel.ForEach(s => plan.DeleteColumn(s, tableName));

                // переименовываем колонку
                plan.RenameColumn(rename.old, rename.actual, tableName);
            }
            else
            {
                var schemas = old.GetOrCreateSchema(context)
                    .FullJoin(
                        actual.GetOrCreateSchema(context),
                        x => x.FullName,
                        x => new { old = x, actual = default(ColumnSchemaDefinition) },
                        x => new { old = default(ColumnSchemaDefinition), actual = x },
                        (x, y) => new { old = x, actual = y });

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
                        //if old or actual types are references
                        if (schema.old.Type.IsReference ||
                            schema.actual.Type.IsReference)
                        {
                            if (schema.old.Type != schema.actual.Type)
                                plan.AlterColumn(schema.actual, tableName);
                        }
                        else if (schema.old.Type != schema.actual.Type)
                            plan.AlterColumn(schema.actual, tableName);
                    }
                }
            }
        }
    }
}