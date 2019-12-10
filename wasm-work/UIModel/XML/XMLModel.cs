using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace UIModel.XML
{
    public abstract class Control
    {
    }

    public abstract class Container : Control
    {
        protected Container()
        {
            Controls = new List<Control>();
        }


        [XmlArray("Controls")]
        [XmlArrayItem(typeof(Button))]
        [XmlArrayItem(typeof(Field))]
        public List<Control> Controls { get; }
    }

    public class Form : Container
    {
    }

    public class Panel : Container
    {
    }

    public class Binding
    {
        public string Path { get; set; }
    }

    public enum FieldType
    {
        Text,
        Date,
        Integer
    }

    [Serializable, XmlRoot("Field")]
    public class Field : Control
    {
        public Field()
        {
            Bindings = new List<Binding>();
        }

        [XmlAttribute] public FieldType Type { get; set; }

        [XmlAttribute] public string Value { get; set; }

        [XmlArray] public List<Binding> Bindings { get; set; }
    }

    [Serializable, XmlRoot("Button")]
    public class Button : Control
    {
        [XmlAttribute] public string OnClick { get; set; }

        [XmlAttribute] public string Text { get; set; }
    }
}