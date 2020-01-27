using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Contracts.Migration
{
    public interface IEntityMigrationScope: IEnumerable<IEntityMigrationItem>
    {
        void AddColumn(XCColumnSchemaDefinition schema, string tableName);
        void AlterColumn(XCColumnSchemaDefinition schema, string tableName);
        void CopyTable(string src, string dst);
        void CreateTable(string tableName, IEnumerable<XCColumnSchemaDefinition> schemaDefinitions);
        void DeleteColumn(XCColumnSchemaDefinition schema, string tableName);
        void DeleteTable(string tableName);
        void RenameColumn(XCColumnSchemaDefinition src, XCColumnSchemaDefinition dst, string tableName);
        void RenameTable(string src, string dst);
        void SetFlagChangeTable(string tablename);
        void SetFlagCopyTable(string src, string dst);
        void SetFlagDeleteTable(string tablename);
        void SetFlagRenameTable(string tablename);
        void UpdateType(IProperty property, string tableName, IType type);
    }
}