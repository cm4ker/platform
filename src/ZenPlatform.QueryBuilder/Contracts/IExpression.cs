using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Contracts
{
    public interface IExpression
    {
        SSyntaxNode Expression { get; }
    }
}
