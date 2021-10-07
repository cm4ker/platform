using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(202109242135)]
    public class AddPendingFilesTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table("files_pending")
                .WithColumn("type").AsInt32()
                .WithColumn("name").AsString(200)
                .WithColumn("create_datetime").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("data").AsCustom("VARBINARY(MAX)");
        }

        public override void Down()
        {
            Delete.Table("files_pending");
        }
    }
}