using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class LogicStack : Stack<object>
    {
        public XCComponent PopComponent()
        {
            return (XCComponent) this.Pop();
        }

        public IQDataSource PopDataSource()
        {
            return (IQDataSource) this.Pop();
        }

        public QField PopField()
        {
            return (QField) this.Pop();
        }

        public QExpression PopExpression()
        {
            return (QExpression) this.Pop();
        }
    }
}