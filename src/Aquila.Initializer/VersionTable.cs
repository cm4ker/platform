using FluentMigrator.Runner.VersionTableInfo;

namespace Aquila.Initializer
{
    [VersionTableMetaData]
    public class VersionTable : IVersionTableMetaData
    {
        public string ColumnName
        {
            get { return "version"; }
        }

        public object ApplicationContext { get; set; }

        public bool OwnsSchema { get; }

        public string SchemaName
        {
            get { return ""; }
        }

        public string TableName
        {
            get { return "version"; }
        }

        public string UniqueIndexName
        {
            get { return "id"; }
        }

        public virtual string AppliedOnColumnName
        {
            get { return "applied_on"; }
        }

        public virtual string DescriptionColumnName
        {
            get { return "description"; }
        }
    }
}