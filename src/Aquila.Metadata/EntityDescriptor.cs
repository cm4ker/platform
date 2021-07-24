using System.Diagnostics.CodeAnalysis;

namespace Aquila.Metadata
{
    /// <summary>
    /// Describes current entity state in database
    /// </summary>
    public class EntityDescriptor
    {
        private string _metadataId;
        private string _databaseName;

        public EntityDescriptor(int dbId)
        {
            DatabaseId = dbId;

            _metadataId = "";
            _databaseName = "";
        }

        public int DatabaseId { get; }

        public string MetadataId
        {
            get => _metadataId;
            set => _metadataId = value;
        }

        public string DatabaseName

        {
            get => _databaseName;
            set => _databaseName = value;
        }
    }

    /*
     
     Entity.Invoice       - Tbl_001
     Entity.Invoice.Goods - Tbl_002
     Sales                - Tbl_003
     Sales<Totals>        - Tbl_004
     Sales<Something>     - Tbl_005     
     
     */
}