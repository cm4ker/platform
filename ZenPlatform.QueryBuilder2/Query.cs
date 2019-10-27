using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Builders.Create;
using ZenPlatform.QueryBuilder.Contracts;

namespace ZenPlatform.QueryBuilder
{
    public static class Query
    {
        public static ICreateExpressionRoot Create()
        {
            return new CreateExpressionRoot();
        }
        public static IAlterExpressionRoot Alter()
        {
            throw new NotImplementedException();
        }
        public static IDeleteExpressionRoot Delete()
        {
            throw new NotImplementedException();
        }

    }
}
