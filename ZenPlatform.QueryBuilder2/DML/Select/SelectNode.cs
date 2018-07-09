using System;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Tokens;
using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public class SelectNode : Node, ISelect
    {
        public bool HasFields = false;
        public TopNode _top;

        public SelectNode()
        {
            Childs.Add(Tokens.SelectToken);
            Childs.Add(Tokens.SpaceToken);
        }

        public SelectNode WithTop(int count)
        {
            _top = new TopNode(count);
            Childs.Insert(2, _top);

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

        Node ISelect.TopNode => _top;
    }


    public interface ISelect : IChildItem<Node>, IParentItem<Node, Node>
    {
        Node TopNode { get; }
    }
}