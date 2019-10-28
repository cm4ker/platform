using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Common;

namespace ZenPlatform.QueryBuilder
{
    public interface ISqlCompiler
    {
        string Compile(SqlNode node);
    }
}
