using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
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

    public class UXGroup : UXElement
    {
        private Grid _g;
        private List<UXElement> _elements;

        public UXGroup()
        {
            _elements = new List<UXElement>();
            _g = new Grid();
        }

        public UXGroupOrientation Orientation { get; set; }

        private void UpdateOrientation()
        {
            foreach (var element in _elements)
            {
            }
        }

        public void Add(UXElement element)
        {
            _elements.Add(element);

            var underlyingControl = element.GetUnderlyingControl();

            var index = _elements.Count - 1;


            if (Orientation == UXGroupOrientation.Horizontal)
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