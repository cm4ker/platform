using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Aquila.Core.Querying.Model
{
    public abstract partial class QLangElement
    {
        private QLangElement _parent;

        public QLangElement()
        {
            AttachedPropery = new Dictionary<string, object>();
        }

        public QLangElement Parent => _parent;

        public Dictionary<string, object> AttachedPropery { get; }

        internal void UpdateTree(QLangElement parent = null)
        {
            _parent = parent;
            foreach (var child in GetChildren())
            {
                child.UpdateTree(this);
            }
        }

        public abstract T Accept<T>(QLangVisitorBase<T> visitor);

        public abstract void Accept(QLangVisitorBase visitor);

        public abstract IEnumerable<QLangElement> GetChildren();

        public override string ToString()
        {
            return "";
        }

        protected virtual bool PrintMembers(StringBuilder builder)
        {
            return true;
        }

        public IEnumerable<T> Find<T>()
        {
            foreach (var child in this.GetChildren())
            {
                if (child is T c)
                    yield return c;

                foreach (var nested in child.Find<T>())
                {
                    yield return nested;
                }
            }
        }
    }

    public abstract class QLangCollection : QLangElement, IEnumerable
    {
        private readonly List<QLangElement> _elements;

        public QLangCollection(ImmutableArray<QLangElement> elements) : base()
        {
            _elements = elements.ToList();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<QLangElement>)_elements).GetEnumerator();
        }

        public abstract QLangCollection Add(QLangElement element);
    }

    public static class EnumerableHelper
    {
        public static bool Any(this IEnumerable source)
        {
            IEnumerator e = source.GetEnumerator();
            return e.MoveNext();
        }
    }

    public abstract class QLangCollection<T> : QLangCollection, IEnumerable<T>
        where T : QLangElement
    {
        private readonly ImmutableArray<T> _elements;

        protected QLangCollection(ImmutableArray<T> elements) : base(elements.CastArray<QLangElement>())
        {
            _elements = elements;
        }

        public T this[int index]
        {
            get { return _elements[index]; }
        }

        public ImmutableArray<T> Elements
        {
            get => _elements;
            init => _elements = value;
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_elements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override IEnumerable<QLangElement> GetChildren()
        {
            return _elements;
        }
    }
}