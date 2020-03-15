using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Controls;

namespace ZenPlatform.Avalonia.Wrapper
{
    public class UXGroupCollection : IList<UXElement>
    {
        private readonly UXGroup _gr;
        private Grid _g;

        private List<UXElement> _elements;

        public UXGroupCollection(UXGroup gr)
        {
            _gr = gr;
            _g = (Grid) gr.GetUnderlyingControl();
            _elements = new List<UXElement>();
        }

        public IEnumerator<UXElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(UXElement item)
        {
            var underlyingControl = item.GetUnderlyingControl();
            _elements.Add(item);
            
            
            var index = _elements.Count - 1;

            if (_gr.Orientation == UXGroupOrientation.Horizontal)
            {
                _g.ColumnDefinitions.Add(new ColumnDefinition());
                Grid.SetColumn(underlyingControl, index);
            }
            else
            {
                _g.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(underlyingControl, index);
            }

            _g.Children.Add(underlyingControl);
        }

        public void Clear()
        {
            _elements.Clear();
        }

        public bool Contains(UXElement item)
        {
            return _elements.Contains(item ?? throw new ArgumentNullException(nameof(item)));
        }

        public void CopyTo(UXElement[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public bool Remove(UXElement item)
        {
            return false;
        }

        public int Count => _elements.Count;
        public bool IsReadOnly => false;

        public int IndexOf(UXElement item)
        {
            return _elements.IndexOf(item);
        }

        public void Insert(int index, UXElement item)
        {
        }

        public void RemoveAt(int index)
        {
        }

        public UXElement this[int index]
        {
            get => _elements[index];
            set => throw new NotImplementedException();
        }
    }
}