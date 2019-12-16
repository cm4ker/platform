using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Avalonia;

namespace UIModel.XML
{
    public abstract class Control : AvaloniaObject
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
        [XmlArray("Data")]
        [XmlArrayItem("Object", typeof(ContextObject))]
        public List<ContextObject> ContextObject { get; set; }
    }

    public class Panel : Container
    {
    }

    [Serializable, XmlRoot("Binding")]
    public class ModelBinding
    {
        [XmlAttribute] public string Path { get; set; }

        [XmlAttribute] public bool IsReadOnly { get; set; }
    }

    public enum FieldType
    {
        TypeSelect,
        Text,
        Date,
        Integer,
        Object
    }

    [Serializable, XmlRoot("Field")]
    public class Field : Control
    {
        public Field()
        {
            Bindings = new List<ModelBinding>();
        }

        /// <summary>
        /// Тип поля
        /// </summary>
        [XmlAttribute]
        public FieldType Type { get; set; }


        /// <summary>
        /// Типы которые поле обслуживает, для
        /// FiledType, Object и TypeSelect могут быть несколько типов
        /// </summary>
        [XmlAttribute]
        public List<int> ServingTypes { get; set; }

        /// <summary>
        /// Значение по умолчанию для значимых типов
        /// В остальных случаях необходимо сетить значения по умолчанию из кода
        /// </summary>
        [XmlAttribute]
        public string DefaultValue { get; set; }


        /// <summary>
        /// Связки
        /// </summary>
        [XmlArray]
        public List<ModelBinding> Bindings { get; set; }
    }

    [Serializable, XmlRoot("Button")]
    public class Button : Control
    {
        [XmlAttribute] public string OnClick { get; set; }

        [XmlAttribute] public string Text { get; set; }
    }

    public class ContextObject
    {
        [XmlAttribute] public string Type { get; set; }

        [XmlAttribute] public string Name { get; set; }
    }
}