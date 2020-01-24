using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public class MDType : Metadata, IMDType
    {
        public virtual string Name { get; set; }

        public virtual string RelTableName { get; set; }
    }
}