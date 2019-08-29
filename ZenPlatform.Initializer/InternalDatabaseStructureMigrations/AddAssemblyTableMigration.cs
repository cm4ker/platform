using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(201908041708)]
    public class AddAssemblyTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table("assemblies")
               .WithColumn("id").AsInt32().PrimaryKey().Identity()
               .WithColumn("assembly_hash").AsString(200)
               .WithColumn("configuration_hash").AsString(200)
               .WithColumn("create_datetime").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
               .WithColumn("name").AsString(200)
               .WithColumn("data").AsBinary();

        }

        public override void Down()
        {
            Delete.Table("assemblies");
        }
    }
}
