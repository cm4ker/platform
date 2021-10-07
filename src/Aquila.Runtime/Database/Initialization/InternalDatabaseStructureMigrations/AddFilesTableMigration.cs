using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(201908041708)]
    public class AddFilesTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table("files")
                .WithColumn("type").AsInt32()
                .WithColumn("name").AsString(200)
                .WithColumn("create_datetime").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("data").AsCustom("VARBINARY(MAX)");
        }

        public override void Down()
        {
            Delete.Table("files");
        }
    }
}