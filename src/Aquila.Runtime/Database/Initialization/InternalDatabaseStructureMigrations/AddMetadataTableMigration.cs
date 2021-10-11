using System;
using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(201808031254)]
    public class AddMetadataTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table(DBConstNames.Metadata.MD_TABLE)
                .WithColumn(DBConstNames.Metadata.BLOB_NAME_COLUMN).AsString(200)
                .WithColumn(DBConstNames.Metadata.DATA_COLUMN).AsBinary(Int32.MaxValue);

            Create.Table(DBConstNames.Metadata.PENDING_MD_TABLE)
                .WithColumn(DBConstNames.Metadata.BLOB_NAME_COLUMN).AsString(200)
                .WithColumn(DBConstNames.Metadata.DATA_COLUMN).AsBinary(Int32.MaxValue);
        }

        public override void Down()
        {
            Delete.Table(DBConstNames.Metadata.MD_TABLE);
            Delete.Table(DBConstNames.Metadata.PENDING_MD_TABLE);
        }
    }
}