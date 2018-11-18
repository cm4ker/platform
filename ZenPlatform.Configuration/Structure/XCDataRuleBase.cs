using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    public class XCDataRuleBase : IChildItem<XCDataRuleContent>
    {
        private XCDataRuleContent _parent;

        public XCDataRuleBase()
        {
        }

        public XCDataRuleContent Parent => _parent;

        XCDataRuleContent IChildItem<XCDataRuleContent>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}