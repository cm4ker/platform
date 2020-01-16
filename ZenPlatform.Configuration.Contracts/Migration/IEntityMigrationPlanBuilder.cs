using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Builders;

namespace ZenPlatform.Configuration.Contracts.Migration
{
    public interface IEntityMigrationPlanBuilder
    {
        void AddColumnItemVisit(DDLQuery query, AddColumnItem item);
        void AlterColumnItemVisit(DDLQuery query, AlterColumnItem item);
        void Build(IEnumerable<IEntityMigrationItem> plan, DDLQuery query);
        void CopyTableItemVisit(DDLQuery query, CopyTableItem item);
        void CreateTableItemVisit(DDLQuery query, CreateTableItem item);
        void DeleteColumnItemVisit(DDLQuery query, DeleteColumnItem item);
        void DeleteTableItemVisit(DDLQuery query, DeleteTableItem item);
        void RenameColumnItemVisit(DDLQuery query, RenameColumnItem item);
        void RenameTableItemVisit(DDLQuery query, RenameTableItem item);
        void SetFlagChangeTableItemVisit(DDLQuery query, SetFlagChangeTableItem item);
        void SetFlagCopyTableItemVisit(DDLQuery query, SetFlagCopyTableItem item);
        void SetFlagDeleteTableItemVisit(DDLQuery query, SetFlagDeleteTableItem item);
        void SetFlagRenameTableItemVisit(DDLQuery query, SetFlagRenameTableItem item);
        void UpdateTypeItemVisit(DDLQuery query, UpdateTypeItem item);
    }
}