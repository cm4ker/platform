using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Contracts.Migration
{
    public interface IEntityMigrationScope: IEnumerable<IEntityMigrationItem>
    {
        void AddColumn(ColumnSchemaDefinition schema, string tableName);
        void AlterColumn(ColumnSchemaDefinition schema, string tableName);
        void CopyTable(string src, string dst);
        void CreateTable(string tableName, IEnumerable<ColumnSchemaDefinition> schemaDefinitions);
        void DeleteColumn(ColumnSchemaDefinition schema, string tableName);
        void DeleteTable(string tableName);
        void RenameColumn(ColumnSchemaDefinition src, ColumnSchemaDefinition dst, string tableName);
        void RenameTable(string src, string dst);
        void SetFlagChangeTable(string tablename);
        void SetFlagCopyTable(string src, string dst);
        void SetFlagDeleteTable(string tablename);
        void SetFlagRenameTable(string tablename);
        void UpdateType(IProperty property, string tableName, IType type);
    }
}