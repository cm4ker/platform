using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Common;

namespace Aquila.QueryBuilder
{
    public interface ISqlCompiler
    {
        string Compile(SqlNode node);
    }
}
