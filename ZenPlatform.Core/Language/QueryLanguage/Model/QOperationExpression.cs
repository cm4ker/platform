using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    public class QOperationExpression : QExpression
    {
        public QOperationExpression()
        {
            Arguments = new List<QExpression>();
        }

        protected List<QExpression> Arguments { get; }

        protected virtual int ParamCount => throw new NotImplementedException();

        public void PushArgument(QExpression argument)
        {
            if (ParamCount != 0)
                if (Arguments.Count == ParamCount)
                    throw new Exception("Enough params today");

            Arguments.Add(argument);
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield return new XCBoolean();
        }
    }
}