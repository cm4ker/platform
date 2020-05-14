using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    // [Migration(201808031254)]
    // public class AddConfigTableMigration : Migration
    // {
    //     public override void Up()
    //     {
    //         Create.Table(DatabaseConstantNames.CONFIG_TABLE_NAME)
    //             .WithColumn(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD).AsString(200)
    //             .WithColumn(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD).AsCustom("VARBINARY(MAX)");
    //
    //         Create.Table(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME)
    //             .WithColumn(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD).AsString(200)
    //             .WithColumn(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD).AsCustom("VARBINARY(MAX)");
    //     }
    //
    //     public override void Down()
    //     {
    //         Delete.Table(DatabaseConstantNames.CONFIG_TABLE_NAME);
    //         Delete.Table(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME);
    //     }
    // }
}