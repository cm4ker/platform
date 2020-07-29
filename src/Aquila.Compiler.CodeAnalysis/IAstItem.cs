using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Language.Ast.Misc;

namespace Aquila.Language.Ast
{
    public class SpanInfo : ILineInfo
    {
        public int Line { get; set; }
        public int Position { get; set; }

        public int StartIndex { get; set; }

        public int StopIndex { get; set; }

        public int Length => StopIndex - StartIndex;
    }

    public class UnknownSpanInfo : SpanInfo
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
}