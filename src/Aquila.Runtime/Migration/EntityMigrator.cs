using System;
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
        private bool CheckDBChangesMainObject(SMEntity x, SMEntity y)
        {
            return !x.Properties.SequenceEqual(y.Properties);
        }

        private bool CheckDBChangesTable(SMTable x, SMTable y)
        {
            return !x.Properties.SequenceEqual(y.Properties);
        }

        private string GetDbName(SMEntity md, EntityMigratorDataContext context)
        {
            var mdId = md.FullName;
            var descriptor = context.RuntimeContext.Descriptors.GetEntityDescriptor(mdId);

            if (descriptor == null)
            {
                descriptor = context.RuntimeContext.Descriptors.CreateDescriptor(context.ConnectionContext);
                descriptor.DatabaseName = $"Tbl_{descriptor.DatabaseId}";
                descriptor.MetadataId = mdId;
                context.RuntimeContext.SaveAll(context.ConnectionContext);
            }

            return descriptor.DatabaseName;
        }

        private string GetDbName(SMTable md, EntityMigratorDataContext context)
        {
            var mdId = md.FullName;
            return GetDbName(mdId, context);
        }

        private string GetDbName(string mdId, EntityMigratorDataContext context)
        {
            var descriptor = context.RuntimeContext.Descriptors.GetEntityDescriptor(mdId);

            if (descriptor == null)
            {
                descriptor = context.RuntimeContext.Descriptors.CreateDescriptor(context.ConnectionContext);
                descriptor.DatabaseName = $"Tbl_{descriptor.DatabaseId}";
                descriptor.MetadataId = mdId;
                context.RuntimeContext.SaveAll(context.ConnectionContext);
            }

            return descriptor.DatabaseName;
        }


        private void ChangeDatabaseTable(EntityMigrationScope scope, SMEntity old, SMEntity actual,
            EntityMigratorDataContext context)
        {
            string tableName = $"{GetDbName(actual, context)}_tmp";

            //var task = new MigrationTaskAction(step, $"Change table {tableName}");

            AnalyzeProperties(scope, old.Properties, actual.Properties, tableName, context);
        }


        private void ChangeDatabaseTable(EntityMigrationScope scope, SMTable old, SMTable actual,
            EntityMigratorDataContext context)
        {
            string tableName = $"{GetDbName(actual, context)}_tmp";

            //var task = new MigrationTaskAction(step, $"Change table {tableName}");

            AnalyzeProperties(scope, old.Properties, actual.Properties, tableName, context);
        }

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
                    DeleteProperty(scope, property.old, tableName, context.Current, context);
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
            var oldState = context.Current.GetSemanticMetadata();
            var actualState = context.Pending.GetSemanticMetadata();

            if (!oldState.Any() && actualState.Any())
            {
                var typesToCreate = context.Pending.GetSemanticMetadata();

                typesToCreate.ForEach(e =>
                    plan.AddScope(
                        scope =>
                        {
                            scope.CreateTable(GetDbName(e, context),
                                e.Properties.SelectMany(p =>
                                    p.GetOrCreateSchema(context)));

                            foreach (var table in e.Tables)
                            {
                                scope.CreateTable(GetDbName(table, context),
                                    table.Properties.SelectMany(x => x.GetOrCreateSchema(context)));
                            }
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


                                foreach (var table in actual.Tables)
                                {
                                    scope.CreateTable(GetDbName(table, context),
                                        table.Properties.SelectMany(x => x.GetOrCreateSchema(context)));
                                }
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

                        var oldTabes = old.Tables;
                        var actualTables = actual.Tables;

                        var tables = oldTabes.FullJoin(actualTables, x => x.FullName,
                            x => new { old = x, actual = default(SMTable) },
                            x => new { old = default(SMTable), actual = x },
                            (x, y) => new { old = x, actual = y });

                        foreach (var t in tables)
                        {
                            if (CheckDBChangesTable(t.old, t.actual))
                            {
                                MigrateTable(plan, t.old, t.actual, context);
                            }
                        }
                    }
                }
            }
        }


        private void MigrateTable(EntityMigrationPlan plan, SMTable old, SMTable actual,
            EntityMigratorDataContext context)
        {
            var actualDbName = GetDbName(actual, context);
            var oldDbName = GetDbName(old, context);

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

        public void CreateProperty(EntityMigrationScope plan, SMProperty property, string tableName,
            EntityMigratorDataContext context)
        {
            foreach (var col in property.GetOrCreateSchema(context))
            {
                plan.AddColumn(col, tableName);
            }
        }

        public void DeleteProperty(EntityMigrationScope plan, SMProperty property, string tableName,
            MetadataProvider mdCol, EntityMigratorDataContext context)
        {
            property.GetOrCreateSchema(context).ForEach(s => plan.DeleteColumn(s, tableName));
        }

        public void ChangeTable(EntityMigrationScope plan, SMTable old, SMTable actual, string tableName,
            EntityMigratorDataContext context)
        {
            AnalyzeProperties(plan, old.Properties, old.Properties, tableName, context);
        }

        public void ChangeProperty(EntityMigrationScope plan, SMProperty old, SMProperty actual,
            string tableName, EntityMigratorDataContext context)
        {
            var oldMdCol = context.Current;
            var actualMdCol = context.Pending;

            //case then count types 1 => many
            if (old.Types.Count() == 1 && actual.Types.Count() > 1)
            {
                //get rename column
                var rename = old.GetOrCreateSchema(context).Join(
                    actual.GetOrCreateSchema(context),
                    o => o.Type,
                    a => a.Type,
                    (x, y) => new { old = x, actual = y }
                ).FirstOrDefault();

                if (rename is null)
                {
                    throw new Exception($"Column {old.Name} can't handle");
                }

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