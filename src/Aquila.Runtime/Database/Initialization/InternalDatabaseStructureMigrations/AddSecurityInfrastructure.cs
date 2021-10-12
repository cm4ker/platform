using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(202110111659)]
    public class AddSecurityInfrastructure : Migration
    {
        public override void Up()
        {
            Create.Table(DBConstNames.Security.SECURITY_TABLE)
                .WithColumn(DBConstNames.Security.ID_COLUMN).AsInt32()
                .WithColumn(DBConstNames.Security.MD_NAME_COLUMN).AsString(1000)
                .WithColumn(DBConstNames.Security.RIGHTS_COLUMN).AsInt32();
        }

        public override void Down()
        {
            Delete.Table("files_pending");
        }
    }
}