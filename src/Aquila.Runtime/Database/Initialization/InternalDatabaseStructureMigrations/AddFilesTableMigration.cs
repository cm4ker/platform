using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(201908041708)]
    public class AddFilesTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table(DBConstNames.Files.FILES_TABLE)
                .WithColumn(DBConstNames.Files.TYPE_COLUMN).AsInt32()
                .WithColumn(DBConstNames.Files.NAME_COLUMN).AsString(200)
                .WithColumn(DBConstNames.Files.CREATE_DATETIME_COLUMN).AsDateTime()
                .WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn(DBConstNames.Files.DATA_COLUMN).AsCustom("VARBINARY(MAX)");

            Create.Table(DBConstNames.Files.PENDING_FILES_TABLE)
                .WithColumn(DBConstNames.Files.TYPE_COLUMN).AsInt32()
                .WithColumn(DBConstNames.Files.NAME_COLUMN).AsString(200)
                .WithColumn(DBConstNames.Files.CREATE_DATETIME_COLUMN).AsDateTime()
                .WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn(DBConstNames.Files.DATA_COLUMN).AsCustom("VARBINARY(MAX)");
        }

        public override void Down()
        {
            Delete.Table(DBConstNames.Files.FILES_TABLE);
            Delete.Table(DBConstNames.Files.PENDING_FILES_TABLE);
        }
    }
}