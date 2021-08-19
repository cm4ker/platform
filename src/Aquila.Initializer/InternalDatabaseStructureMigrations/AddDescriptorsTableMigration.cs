using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(202107121615)]
    public class AddDescriptorsTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table("descriptors")
                .WithColumn("id").AsInt32().PrimaryKey().Identity(100, 1)
                .WithColumn("id_s").AsString(300)
                .WithColumn("db_name").AsString(150);
        }

        public override void Down()
        {
            Delete.Table("descriptors");
        }
    }
}