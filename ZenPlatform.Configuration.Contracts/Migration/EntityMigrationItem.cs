using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.QueryBuilder.Builders;

namespace ZenPlatform.Configuration.Contracts.Migration
{
    public interface IEntityMigrationItem
    {
        void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query);
    }

    public class AddColumnItem : IEntityMigrationItem
    {
        public XCColumnSchemaDefinition Schema { get; }
        public string TableName { get; }

        public AddColumnItem(XCColumnSchemaDefinition schema, string tableName)
        {
            Schema = schema;
            TableName = tableName;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.AddColumnItemVisit(query, this);
        }
    }

    public class AlterColumnItem : IEntityMigrationItem
    {
        public XCColumnSchemaDefinition Schema { get; }
        public string TableName { get; }

        public AlterColumnItem(XCColumnSchemaDefinition schema, string tableName)
        {
            Schema = schema;
            TableName = tableName;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.AlterColumnItemVisit(query, this);
        }
    }

    public class DeleteColumnItem : IEntityMigrationItem
    {
        public XCColumnSchemaDefinition Schema { get; }
        public string TableName { get; }

        public DeleteColumnItem(XCColumnSchemaDefinition schema, string tableName)
        {
            Schema = schema;
            TableName = tableName;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.DeleteColumnItemVisit(query, this);
        }
    }

    public class UpdateTypeItem : IEntityMigrationItem
    {
        public IProperty Property { get; }
        public string TableName { get; }

        public IType XCType { get; }

        public UpdateTypeItem(IProperty property, string tableName, IType type)
        {
            Property = property;
            TableName = tableName;
            XCType = type;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.UpdateTypeItemVisit(query, this);
        }
    }

    public class RenameColumnItem : IEntityMigrationItem
    {
        public XCColumnSchemaDefinition SrcSchema { get; }
        public XCColumnSchemaDefinition DstSchema { get; }

        public string TableName { get; }

        public RenameColumnItem(XCColumnSchemaDefinition src, XCColumnSchemaDefinition dst, string tableName)
        {
            SrcSchema = src;
            DstSchema = dst;
            TableName = tableName;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.RenameColumnItemVisit(query, this);
        }
    }

    public class DeleteTableItem : IEntityMigrationItem
    {
        public string TableName { get; }

        public DeleteTableItem(string tableName)
        {
            TableName = tableName;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.DeleteTableItemVisit(query, this);
        }
    }

    public class RenameTableItem : IEntityMigrationItem
    {
        public string SrcTableName { get; }
        public string DstTableName { get; }

        public RenameTableItem(string src, string dst)
        {
            SrcTableName = src;
            DstTableName = dst;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.RenameTableItemVisit(query, this);
        }
    }

    public class CopyTableItem : IEntityMigrationItem
    {
        public string SrcTableName { get; }
        public string DstTableName { get; }

        public CopyTableItem(string src, string dst)
        {
            SrcTableName = src;
            DstTableName = dst;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.CopyTableItemVisit(query, this);
        }
    }


    public class CreateTableItem : IEntityMigrationItem
    {
        public string TableName { get; }
        public IEnumerable<XCColumnSchemaDefinition> SchemaDefinitions;

        public CreateTableItem()
        {
        }

        public CreateTableItem(string tableName, IEnumerable<XCColumnSchemaDefinition> schemaDefinitions)
        {
            TableName = tableName;
            SchemaDefinitions = schemaDefinitions;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.CreateTableItemVisit(query, this);
        }
    }

    public class SetFlagCopyTableItem : IEntityMigrationItem
    {
        public string SrcTableName { get; }
        public string DstTableName { get; }

        public SetFlagCopyTableItem(string src, string dst)
        {
            SrcTableName = src;
            DstTableName = dst;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.SetFlagCopyTableItemVisit(query, this);
        }
    }

    public class SetFlagChangeTableItem : IEntityMigrationItem
    {
        public string TableName { get; }

        public SetFlagChangeTableItem(string tableName)
        {
            TableName = tableName;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.SetFlagChangeTableItemVisit(query, this);
        }
    }


    public class SetFlagDeleteTableItem : IEntityMigrationItem
    {
        public string TableName { get; }

        public SetFlagDeleteTableItem(string tableName)
        {
            TableName = tableName;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.SetFlagDeleteTableItemVisit(query, this);
        }
    }

    public class SetFlagRenameTableItem : IEntityMigrationItem
    {
        public string TableName { get; }

        public SetFlagRenameTableItem(string tableName)
        {
            TableName = tableName;
        }

        public void Visit(IEntityMigrationPlanBuilder visitor, DDLQuery query)
        {
            visitor.SetFlagRenameTableItemVisit(query, this);
        }
    }
}