using System;
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

        public ObjectPickerField()
        {
            _htmlInput = D.Doc.CreateElement<HTMLInputElement>();
            _htmlClearButton = D.Doc.CreateElement<HTMLButtonElement>();
            _htmlLookupButton = D.Doc.CreateElement<HTMLButtonElement>();
            _htmlSelectTypeButton = D.Doc.CreateElement<HTMLButtonElement>();
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
            set { SetAndRaise(ValueProperty, ref _value, value); }
        }
    }
}