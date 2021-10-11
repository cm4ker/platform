using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(202107121615)]
    public class AddDescriptorsTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table(DBConstNames.Descriptors.DESCRIPTORS_TABLE)
                .WithColumn(DBConstNames.Descriptors.ID_COLUMN).AsInt32().PrimaryKey().Identity()
                .WithColumn(DBConstNames.Descriptors.MD_NAME_COLUMN).AsString(300)
                .WithColumn(DBConstNames.Descriptors.DB_ID_COLUMN).AsInt32()
                .WithColumn(DBConstNames.Descriptors.DB_NAME_COLUMN).AsString(150);
        }

        public override void Down()
        {
            Delete.Table(DBConstNames.Descriptors.DESCRIPTORS_TABLE);
        }
    }
}