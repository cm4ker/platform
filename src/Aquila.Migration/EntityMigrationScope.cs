using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.Migration;
using Aquila.Configuration.Contracts.TypeSystem;

namespace Aquila.Migration
{
    public class EntityMigrationScope : List<IEntityMigrationItem>, IEntityMigrationScope
    {
        public void AddColumn(ColumnSchemaDefinition schema, string tableName)
        {
            Add(new AddColumnItem(schema, tableName));
        }

        public void AlterColumn(ColumnSchemaDefinition schema, string tableName)
        {
            Add(new AlterColumnItem(schema, tableName));
        }

        public void DeleteColumn(ColumnSchemaDefinition schema, string tableName)
        {
            Add(new DeleteColumnItem(schema, tableName));
        }

        public void UpdateType(IPProperty property, string tableName, IPType ipType)
        {
            Add(new UpdateTypeItem(property, tableName, ipType));
        }

        public void RenameColumn(ColumnSchemaDefinition src, ColumnSchemaDefinition dst, string tableName)
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

        public void CreateTable(string tableName, IEnumerable<ColumnSchemaDefinition> schemaDefinitions)
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