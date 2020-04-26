using System.Reflection;
using FluentMigrator;


namespace ZenPlatform.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(201808031249)]
    public class AddUserTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("user_id").AsGuid().PrimaryKey()
                .WithColumn("user_name").AsString(300)
                .WithColumn("is_active").AsBoolean();
        }

        public override void Down()
        {
            Delete.Table("users");
        }
    }
}