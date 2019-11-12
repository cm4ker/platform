using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Quering.Model
{
    /// <summary>
    /// Кейс
    /// </summary>
    public class QCase : QExpression
    {
        public QCase(IList<QCaseWhen> whens)
        {
            Whens = whens;
            if (whens == null || !whens.Any()) throw new Exception("Need at least one element in the conditions");
        }

        public IList<QCaseWhen> Whens { get; }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            foreach (var when in Whens)
            {
                foreach (var typeBase in when.Then.GetRexpressionType())
                {
                    yield return typeBase;
                }

                foreach (var typeBase in when.Else.GetRexpressionType())
                {
                    yield return typeBase;
                }
            }
        }
    }


    public class QCaseWhen : QItem
    {
        public QCaseWhen(QExpression @else, QExpression then, QOperationExpression @when)
        {
            When = when;
            Then = then;
            Else = @else;
        }

        public QOperationExpression When { get; }

        public QExpression Then { get; }

        public QExpression Else { get; }
    }
}