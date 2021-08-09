using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(202108081428)]
    public class AddPackagesTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table("packages")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("date").AsDateTime2()
                .WithColumn("content").AsBinary();
        }

        public override void Down()
        {
            Delete.Table("descriptors");
        }
    }
}