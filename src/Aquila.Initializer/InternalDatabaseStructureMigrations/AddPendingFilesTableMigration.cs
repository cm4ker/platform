using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(202109242135)]
    public class AddPendingFilesTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table("pending_files")
                .WithColumn("type").AsInt32()
                .WithColumn("name").AsString(200)
                .WithColumn("create_datetime").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("data").AsCustom("VARBINARY(MAX)");
        }

        public override void Down()
        {
            Delete.Table("pending_files");
        }
    }
}