using ZenPlatform.Configuration.Common;

namespace ZenPlatform.Configuration.Structure.Data.Types
{
    /// <summary>
    /// Неопределённый тип, при загрузке конфигурации сначала всё приводится к нему
    /// </summary>
    public class XCUnknownType : XCTypeBase
    {
        protected override bool ShouldSerializeDescription()
        {
            return false;
        }

        protected override bool ShouldSerializeName()
        {
            return false;
        }
        
        protected override bool ShouldSerializeId()
        {
            return false;
        }
    }
}