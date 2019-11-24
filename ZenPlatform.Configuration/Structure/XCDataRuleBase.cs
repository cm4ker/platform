using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
   

    public class XCDataRuleBase : IXCDataRule
    {
        private IXCDataRuleContent _parent;

        public XCDataRuleBase()
        {
        }

        public IXCDataRuleContent Parent => _parent;

        IXCDataRuleContent IChildItem<IXCDataRuleContent>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}