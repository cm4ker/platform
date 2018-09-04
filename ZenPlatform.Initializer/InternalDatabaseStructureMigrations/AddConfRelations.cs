using FluentMigrator;

namespace ZenPlatform.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(201809050000)]
    public class AddConfRelations : Migration
    {
        public override void Up()
        {
            Create.Table("conf_relations")
                .WithColumn("conf_object_id").AsGuid()
                .WithColumn("db_object_id").AsInt32();
        }

        public override void Down()
        {
            Delete.Table("conf_relations");
        }
    }
}