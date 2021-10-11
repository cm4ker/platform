using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(202110111659)]
    public class AddSecurityInfrastructure : Migration
    {
        public override void Up()
        {
            Create.Table("sec")
                .WithColumn("id").AsInt32()
                .WithColumn("desc_id_s").AsString(1000);
        }

        public override void Down()
        {
            Delete.Table("files_pending");
        }
    }
}