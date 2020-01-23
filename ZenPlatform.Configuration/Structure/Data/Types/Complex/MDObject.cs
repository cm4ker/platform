using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public abstract class MDObject : Metadata
    {
        public virtual string RelTableName { get; set; }
    }
}