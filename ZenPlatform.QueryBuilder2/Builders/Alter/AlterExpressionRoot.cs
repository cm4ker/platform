using ZenPlatform.QueryBuilder.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.QueryBuilder.Builders
{
    class AlterExpressionRoot : IAlterExpressionRoot
    {
        public IAlterColumnOnTableSyntax Column(string columnName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax Table(string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
