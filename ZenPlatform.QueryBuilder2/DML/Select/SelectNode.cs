using System;
using System.Runtime.Serialization;
using System.Security;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DDL.CreateTable;

namespace ZenPlatform.QueryBuilder2.DML.Select
{
    public class SelectNode : SqlNode
    {
        public bool HasFields = false;

        public SelectNode()
        {
            Childs.Add(Tokens.SelectToken);
            Childs.Add(Tokens.SpaceToken);
        }

        public SelectNode WithTop(int count)
        {
            Childs.Insert(2, Tokens.TopToken);
            Childs.Insert(3, new RawSqlNode(count.ToString()));
            Childs.Insert(4, Tokens.SpaceToken);
            return this;
        }

        public SelectNode Select(string fieldName)
        {
            return Select(fieldName, (f) => { });
        }

        public SelectNode Select(string fieldName, Action<SelectFieldNode> options)
        {
            var f = new SelectFieldNode(fieldName);
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