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

        public object Value { get; set; }
    }
}