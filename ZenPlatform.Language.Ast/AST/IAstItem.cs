using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Language.Ast.AST
{
    public class LineInfo : ILineInfo
    {
        public int Line { get; set; }
        public int Position { get; set; }
    }

    public class UnknownLineInfo : LineInfo
    {
    }

    public class AstNodeCollection : IEnumerable<AstNode>
    {
        private Dictionary<int, AstNode> _nodes;

        public T SetSlot<T>(T astNode, int index) where T : AstNode
        {
            if (!_nodes.ContainsKey(index))
                _nodes.Add(index, astNode);
            else
                _nodes[index] = astNode;

            return astNode;
        }

        public T GetSlot<T>(int index) where T : AstNode
        {
            if (_nodes.TryGetValue(index, out var value))
                return value as T;

            return null;
        }

        public T GetSlot<T>(out T field, int index) where T : AstNode
        {
            field = GetSlot<T>(index);
            return field;
        }

        public IReadOnlyList<AstNode> ToImmutable()
        {
            return _nodes.Select(x => x.Value).ToImmutableList();
        }

        public IEnumerator<AstNode> GetEnumerator()
        {
            return _nodes.OrderBy(x => x.Key).Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public abstract class AstNode : IAstNode
    {
        public AstNode(ILineInfo lineInfo)
        {
            if (lineInfo != null)
            {
                Line = lineInfo.Line;
                Position = lineInfo.Position;
            }

            Children = new AstNodeCollection();
        }

        public int Line { get; set; }

        public int Position { get; set; }

        public IAstNode Parent { get; set; }

        public AstNodeCollection Children { get; }

        public T GetParent<T>() where T : IAstNode
        {
            if (Parent is null) return default(T);

            if (Parent is T p)
                return p;

            return Parent.GetParent<T>();
        }

        public abstract T Accept<T>(AstVisitorBase<T> visitor);
    }
}