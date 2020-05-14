using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(2018049042348)]
    public class AddPasswordFieldToUserTable : Migration
    {
        public override void Up()
        {
            Alter.Table("users").AddColumn("user_password").AsBinary(200);
        }

        public override void Down()
        {
            Delete.Column("user_password").FromTable("users");
        }
    }
}