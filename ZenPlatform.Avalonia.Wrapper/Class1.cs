using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.OpenGL;
using Avalonia.Styling;
using JetBrains.Annotations;

namespace ZenPlatform.Avalonia.Wrapper
{
    /*
        Transformer.Wrap(string) => object
        Transformer.Unwrap(UIElement) => string
    */

    public static class Transformer
    {
        public static UXElement Wrap()
        {
            var form = new UXForm();
            var tb = new UXTextBox();

            form.SetContent(tb);

            return form;
        }
    }

    public class UXReader
    {
        private XmlReader _reader;

        public UXReader()
        {
            _reader = XmlReader.Create(new string("test"));
        }
    }

    /*
     <UXForm> 
        <UXGroup Orient="Horizontal">
            <UXTextBox />
            <UXTextBox />
        </UXGroup> 
     </UXForm>
     */
    public abstract class UXElement
    {
        public abstract Control GetUnderlyingControl();
    }

    public class UXForm : UXElement
    {
        private UserControl _uc;

        public UXForm()
        {
            _uc = new UserControl();
        }

        public void SetContent(UXElement element)
        {
            _uc.Content = element.GetUnderlyingControl();
        }

        public override Control GetUnderlyingControl()
        {
            return _uc;
        }
    }

    public enum UXGroupOrientation
    {
        Horizontal,
        Vertical
    }

    public class UXGroupCollection : IList<UXElement>
    {
        private readonly UXGroup _gr;
        private Grid _g;

        private List<UXElement> _elements;

        public UXGroupCollection(UXGroup gr)
        {
            _gr = gr;
            _g = (Grid) gr.GetUnderlyingControl();
        }

        public IEnumerator<UXElement> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(UXElement item)
        {
            var underlyingControl = item.GetUnderlyingControl();

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
            throw new NotImplementedException();
        }

        public bool Contains(UXElement item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(UXElement[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(UXElement item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; }
        public bool IsReadOnly { get; }

        public int IndexOf(UXElement item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, UXElement item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public UXElement this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    public class UXGroup : UXElement
    {
        private Grid _g;

        public UXGroup()
        {
            _g = new Grid();
        }

        public UXGroupOrientation Orientation { get; set; }

        private void UpdateOrientation()
        {
        }

        public void Add(UXElement element)
        {
        }

        public override Control GetUnderlyingControl()
        {
            return _g;
        }
    }

    public class UXTextBox : UXElement
    {
        private TextBox _c;

        public UXTextBox()
        {
            _c = new TextBox();
        }

        public override Control GetUnderlyingControl()
        {
            return _c;
        }
    }
}