using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Language.Ast
{
    public class LineInfo : ILineInfo
    {
        public int Line { get; set; }
        public int Position { get; set; }
    }

    public class UnknownLineInfo : LineInfo
    {
    }

    public class AstNodeCollection : IEnumerable<SyntaxNode>
    {
        private Dictionary<int, SyntaxNode> _nodes;

        public AstNodeCollection()
        {
            _nodes = new Dictionary<int, SyntaxNode>();
        }

        public T SetSlot<T>(T astNode, int index) where T : SyntaxNode
        {
            if (!_nodes.ContainsKey(index))
            {
                if (astNode is null) return null;

                _nodes.Add(index, astNode);
            }
            else
            {
                if (astNode is null)
                    _nodes.Remove(index);
                else
                    _nodes[index] = astNode;
            }

            return astNode;
        }

        public T GetSlot<T>(int index) where T : SyntaxNode
        {
            if (_nodes.TryGetValue(index, out var value))
                return value as T;

            return null;
        }

        public T GetSlot<T>(out T field, int index) where T : SyntaxNode
        {
            field = GetSlot<T>(index);
            return field;
        }

        public IReadOnlyList<SyntaxNode> ToImmutable()
        {
            return _nodes.Select(x => x.Value).ToImmutableList();
        }

        public IEnumerator<SyntaxNode> GetEnumerator()
        {
            return _nodes.OrderBy(x => x.Key).Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public abstract class SyntaxNode : Node, IAstNode
    {
        public SyntaxNode(ILineInfo lineInfo)
        {
            if (lineInfo != null)
            {
                Line = lineInfo.Line;
                Position = lineInfo.Position;
            }

            //  Children = new AstNodeCollection();
        }

        public int Line { get; set; }

        public int Position { get; set; }

        public abstract T Accept<T>(AstVisitorBase<T> visitor);
    }
}