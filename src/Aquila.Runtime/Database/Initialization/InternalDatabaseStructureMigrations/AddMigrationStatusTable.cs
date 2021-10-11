using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(202001131454)]
    public class AddMigrationStatusTable : Migration
    {
        public override void Up()
        {
            Create.Table(DBConstNames.MigrationStatus.MIGRATION_STATUS_TABLE)
                .WithColumn(DBConstNames.MigrationStatus.ID_COLUMN).AsGuid().NotNullable()
                .WithColumn(DBConstNames.MigrationStatus.CHANGE_TABLE_COLUMN).AsBoolean().WithDefaultValue(false)
                .NotNullable()
                .WithColumn(DBConstNames.MigrationStatus.RENAME_TABLE_COLUMN).AsBoolean().WithDefaultValue(false)
                .NotNullable()
                .WithColumn(DBConstNames.MigrationStatus.DELETE_TABLE_COLUMN).AsBoolean().WithDefaultValue(false)
                .NotNullable()
                .WithColumn(DBConstNames.MigrationStatus.COPY_TABLE_COLUMN).AsBoolean().WithDefaultValue(false)
                .NotNullable()
                .WithColumn(DBConstNames.MigrationStatus.ORIGINAL_TABLE_COLUMN).AsString(200)
                .WithColumn(DBConstNames.MigrationStatus.TEMP_TABLE_COLUMN).AsString(200)
                .WithColumn(DBConstNames.MigrationStatus.DATETIME_COLUMN).AsDateTime()
                .WithDefault(SystemMethods.CurrentDateTime);
            ;

            Create.Table(DBConstNames.Migration.MIGRATION_TABLE)
                .WithColumn(DBConstNames.Migration.ID_COLUMN).AsGuid().PrimaryKey().NotNullable()
                .WithColumn(DBConstNames.Migration.COMPLETED_COLUMN).AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn(DBConstNames.Migration.DATETIME_COLUMN).AsDateTime()
                .WithDefault(SystemMethods.CurrentDateTime);
        }

        public override void Down()
        {
            Delete.Table(DBConstNames.MigrationStatus.MIGRATION_STATUS_TABLE);
            Delete.Table(DBConstNames.Migration.MIGRATION_TABLE);
        }
    }
}