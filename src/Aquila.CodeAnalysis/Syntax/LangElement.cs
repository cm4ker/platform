using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Text;


namespace Aquila.Syntax
{
    public abstract partial record LangElement
    {
        private AquilaSyntaxTree _st;
        private LangElement _parent;

        public LangElement(Span span, SyntaxKind kind)
        {
            Span = span;
            Kind = kind;
        }

        public Span Span { get; init; }
        public SyntaxKind Kind { get; init; }

        public LangElement Parent => _parent;
        public AquilaSyntaxTree SyntaxTree => _st;

        internal void UpdateBelongSyntaxTree(AquilaSyntaxTree st)
        {
            if (_st is not null && st is not null)
                throw new Exception("The lang element already connected with syntax tree");

            _st = st;
            GetChildren().ForEach(x => x.UpdateBelongSyntaxTree(st));
        }

        internal void UpdateTree(LangElement parent = null)
        {
            _parent = parent;
            GetChildren().ForEach(x => x.UpdateTree(this));
        }

        public abstract T Accept<T>(AstVisitorBase<T> visitor);

        public abstract void Accept(AstVisitorBase visitor);

        public abstract IEnumerable<LangElement> GetChildren();

        public override string ToString()
        {
            return "";
        }

        protected virtual bool PrintMembers(StringBuilder builder)
        {
            return true;
        }
    }

    public abstract record LangCollection : LangElement, IEnumerable
    {
        private readonly ImmutableArray<LangElement> _elements;

        public LangCollection(Span span, ImmutableArray<LangElement> elements) : base(span, SyntaxKind.None)
        {
            _elements = elements;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<LangElement>) _elements).GetEnumerator();
        }
    }

    public static class EnumerableHelper
    {
        public static bool Any(this IEnumerable source)
        {
            IEnumerator e = source.GetEnumerator();
            return e.MoveNext();
        }
    }

    public abstract record LangCollection<T> : LangCollection, IEnumerable<T>
        where T : LangElement
    {
        private readonly ImmutableArray<T> _elements;

        protected LangCollection(Span span, ImmutableArray<T> elements) : base(span, elements.CastArray<LangElement>())
        {
            _elements = elements;
        }

        public ImmutableArray<T> Elements
        {
            get => _elements;
            init => _elements = value;
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _elements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            return _elements;
        }
    }
}