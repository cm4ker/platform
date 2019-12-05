using System;
using WebAssembly.Browser.DOM;

namespace UIModel
{
    public abstract class Component
    {
        public virtual Node Element { get; }

        protected Component()
        {
        }
    }

    public class Binding
    {
        public string Path;
    }

    public class Text : Component
    {
        public Text(Document document)
        {
            _underlyingElement = document.CreateElement<HTMLInputElement>();
            _underlyingElement.Type = InputElementType.Text;
            _underlyingElement.OnClick += (sender, args) =>
            {
                ((HTMLInputElement) sender).Value = "HELLLO";
            };
        }

        private HTMLInputElement _underlyingElement;

        public override Node Element => _underlyingElement;

        public string Value { get; set; }
    }

    public class ComboBox : Component
    {
    }

    public class Label : Component
    {
    }

    public class List : Component
    {
    }

    public class Menu : Component
    {
    }

    public class Panel : Component
    {
    }

    public class Button : Component
    {
    }

    public class Grid : Component
    {
    }
}