using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Migration;

namespace ZenPlatform.Migration
{
    public class EntityMigrationScope : List<IEntityMigrationItem>, IEntityMigrationScope
    {

        public void AddColumn(XCColumnSchemaDefinition schema, string tableName)
        {
            Add(new AddColumnItem(schema, tableName));
        }

        public void AlterColumn(XCColumnSchemaDefinition schema, string tableName)
        {
            Add(new AlterColumnItem(schema, tableName));
        }

        public void DeleteColumn(XCColumnSchemaDefinition schema, string tableName)
        {
            Add(new DeleteColumnItem(schema, tableName));
        }

        public void UpdateType(IXProperty property, string tableName, IXCType type)
        {
            Add(new UpdateTypeItem(property, tableName, type));
        }

        public void RenameColumn(XCColumnSchemaDefinition src, XCColumnSchemaDefinition dst, string tableName)
        {
            Add(new RenameColumnItem(src, dst, tableName));
        }


        public void DeleteTable(string tableName)
        {
            Add(new DeleteTableItem(tableName));
        }

        public void RenameTable(string src, string dst)
        {
            Add(new RenameTableItem(src, dst));
        }

        public void CreateTable(string tableName, IEnumerable<XCColumnSchemaDefinition> schemaDefinitions)
        {
            Add(new CreateTableItem(tableName, schemaDefinitions));
        }

        public void CopyTable(string src, string dst)
        {
            Add(new CopyTableItem(src, dst));
        }

        public void SetFlagRenameTable(string tablename)
        {
            Add(new SetFlagRenameTableItem(tablename));
        }

        public void SetFlagDeleteTable(string tablename)
        {
            Add(new SetFlagDeleteTableItem(tablename));
        }

        public void SetFlagChangeTable(string tablename)
        {
            Add(new SetFlagChangeTableItem(tablename));
        }

        public void SetFlagCopyTable(string src, string dst)
        {
            Add(new SetFlagCopyTableItem(src, dst));
        }


    }
}
