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
            Create.Table("migration_status")
               .WithColumn("migration_id").AsGuid().NotNullable()
               .WithColumn("change_table").AsBoolean().WithDefaultValue(false).NotNullable()
               .WithColumn("rename_table").AsBoolean().WithDefaultValue(false).NotNullable()
               .WithColumn("delete_table").AsBoolean().WithDefaultValue(false).NotNullable()
               .WithColumn("copy_table").AsBoolean().WithDefaultValue(false).NotNullable()

               .WithColumn("original_table").AsString(200)
               .WithColumn("temp_table").AsString(200)
               .WithColumn("datetime").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);
            ;

            Create.Table("migrations")
               .WithColumn("migration_id").AsGuid().PrimaryKey().NotNullable()
               .WithColumn("complited").AsBoolean().WithDefaultValue(false).NotNullable()
               .WithColumn("datetime").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);

               ;

        }

        public override void Down()
        {
            Delete.Table("migration_status");
            Delete.Table("migrations");
        }
    }
}
