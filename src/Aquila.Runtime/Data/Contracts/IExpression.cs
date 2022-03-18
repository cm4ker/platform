using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Contracts
{
    public interface IExpression
    {
        SSyntaxNode Expression { get; }
    }
}