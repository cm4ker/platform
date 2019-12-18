using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using Avalonia;
using UIModel.XML;
using WebAssembly.Browser.DOM;

namespace UIModel.HtmlWrapper
{
    internal static class D
    {
        public static Document Doc { get; } = new Document();
    }

    public class Button : AvaloniaObject
    {
    }

    public class Field : AvaloniaObject
    {
        public Field()
        {
        }

        public int Height { get; set; }

        public int Width { get; set; }

        public object Value { get; set; }
    }


    public class UIContainer : StyledElement
    {
    }

    public class ObjectPickerField : AvaloniaObject
    {
        private HTMLInputElement _htmlInput;
        private HTMLButtonElement _htmlClearButton;
        private HTMLButtonElement _htmlLookupButton;
        private HTMLButtonElement _htmlSelectTypeButton;
        private HTMLDivElement _htmlLayout;

        private bool _isPicker = false;

        public HTMLElement Root => _htmlLayout;

        public ObjectPickerField()
        {
            _htmlInput = D.Doc.CreateElement<HTMLInputElement>();
            _htmlClearButton = D.Doc.CreateElement<HTMLButtonElement>();
            _htmlLookupButton = D.Doc.CreateElement<HTMLButtonElement>();
            _htmlSelectTypeButton = D.Doc.CreateElement<HTMLButtonElement>();

            _htmlLayout = D.Doc.CreateElement<HTMLDivElement>();
            _htmlLayout.AppendChild(_htmlInput);
            _htmlLayout.AppendChild(_htmlClearButton);
            _htmlLayout.AppendChild(_htmlLookupButton);
            _htmlLayout.AppendChild(_htmlSelectTypeButton);

            SetPicker(false);
        }

        private void SetPicker(bool isPicker)
        {
            _isPicker = isPicker;

            if (_isPicker)
            {
                // add autocomplete
                
            }
            else
            {
                // _htmlInput.OnKeyup += KeyUp;
                // _htmlInput.OnKeydown += KeyDown;
                // _htmlInput.OnKeypress += KeyPress;
                _htmlInput.OnInput += HtmlInputOnOnInput;
            }
        }

        private void HtmlInputOnOnInput(DOMObject sender, DOMEventArgs args)
        {
            Value = _htmlInput.Value;
        }

        private void KeyPress(DOMObject sender, DOMEventArgs args)
        {
            Value = _htmlInput.Value;
        }

        private void KeyDown(DOMObject sender, DOMEventArgs args)
        {
            Value = _htmlInput.Value;
        }

        private void KeyUp(DOMObject sender, DOMEventArgs args)
        {
            Value = _htmlInput.Value;
        }

        /// <summary>
        /// Defines the <see cref="Text"/> property.
        /// </summary>
        public static readonly DirectProperty<ObjectPickerField, object> ValueProperty =
            AvaloniaProperty.RegisterDirect<ObjectPickerField, object>(
                nameof(Value),
                o => o.Value,
                (o, v) => o.Value = v);

        private object _value;

        public object Value
        {
            get { return _value; }
            set
            {
                SetAndRaise(ValueProperty, ref _value, value);
                _htmlInput.Value = value.ToString();
            }
        }
    }
}