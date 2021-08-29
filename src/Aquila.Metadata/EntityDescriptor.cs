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
        private int _databaseId;


        public EntityDescriptor(int id)
        {
            Id = id;

            _metadataId = "";
            _databaseName = "";

            _databaseId = Id + (int)SMTypeKind.Reference;
        }

        public EntityDescriptor(int id, int dbId) : this(id)
        {
            _databaseId = dbId;
        }

        public int Id { get; }

        public int DatabaseId => _databaseId;

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