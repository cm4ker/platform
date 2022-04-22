using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(202109242113)]
    public class AddDeploysTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table("deploys")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("create_datetime").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("data").AsBinary();
        }

        public override void Down()
        {
            Delete.Table("deploys");
        }
    }
}