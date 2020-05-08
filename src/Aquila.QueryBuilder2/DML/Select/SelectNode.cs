using System;
using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.Common.SqlTokens;
using Aquila.Shared.ParenChildCollection;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.Select
{
    public class SelectNode : SqlNode
    {
        public bool HasFields = false;

        public SelectNode()
        {
        }

        public SelectNode Select(string fieldName)
        {
            return Select(fieldName, (f) => { });
        }

        public SelectNode Select(string fieldName, Action<SelectColumnNode> options)
        {
            var f = new SelectColumnNode(fieldName);
            options(f);

            OnAddField();

            Add(f);

            return this;
        }

        public SelectNode Select(string fieldName, string alias)
        {
            return this.Select(fieldName, (f) => f.As(alias));
        }

        public SelectNode Select(string tableName, string fieldName, string alias)
        {
            return this.Select(fieldName, (f) => f.As(alias).WithTableName(tableName));
        }

        public SelectNode SelectRaw(string raw)
        {
            OnAddField();

            Add(new RawSqlNode(raw));

            return this;
        }

        private void OnAddField()
        {
            if (HasFields)
            {
                Add(Tokens.CommaToken);
                Add(Tokens.SpaceToken);
            }
            else
                HasFields = true;
        }
    }
}