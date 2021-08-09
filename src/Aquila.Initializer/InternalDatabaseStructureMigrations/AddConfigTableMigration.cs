using System;
using FluentMigrator;

namespace Aquila.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(201808031254)]
    public class AddConfigTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table(DatabaseConstantNames.MD_TABLE_NAME)
                .WithColumn(DatabaseConstantNames.MD_TABLE_BLOB_NAME_FIELD).AsString(200)
                .WithColumn(DatabaseConstantNames.MD_TABLE_DATA_FIELD).AsBinary(Int32.MaxValue);

            Create.Table(DatabaseConstantNames.PEND_MD_TABLE_NAME)
                .WithColumn(DatabaseConstantNames.MD_TABLE_BLOB_NAME_FIELD).AsString(200)
                .WithColumn(DatabaseConstantNames.MD_TABLE_DATA_FIELD).AsBinary(Int32.MaxValue);
        }

        public override void Down()
        {
            Delete.Table(DatabaseConstantNames.MD_TABLE_NAME);
            Delete.Table(DatabaseConstantNames.PEND_MD_TABLE_NAME);
        }
    }
}