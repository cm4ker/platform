using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Portable.Xaml;
using ZenPlatform.ServerManagementTool;
using ZenPlatform.Shared.ParenChildCollection;


namespace ZenPlatform.UIGeneration2
{
    public class Program
    {
        public static void Main()
        {
            var context = new XamlSchemaContext(null, null);

            XamlType windowType = new XamlType(typeof(Window), context);

            var heightProperty = new XamlMember(typeof(Window).GetProperty("Height"), context);
            var widthProperty = new XamlMember(typeof(Window).GetProperty("Width"), context);

            var contentProperty = new XamlMember(typeof(Window).GetProperty("Content"), context);

            var sw = new StringWriter();
            var xw = new XamlXmlWriter(sw, context, null);

            xw.WriteStartObject(windowType);
            xw.WriteStartMember(heightProperty);
            xw.WriteValue("100");
            xw.WriteEndMember();

            xw.WriteStartMember(widthProperty);
            xw.WriteValue("2000");
            xw.WriteEndMember();


            xw.WriteStartMember(contentProperty);
            xw.WriteStartObject(windowType);
            xw.WriteEndObject();


            xw.WriteEndMember();

            xw.WriteEndObject();


            xw.Close();

            Console.WriteLine(sw.ToString());
        }
    }

    public class XBNode : IChildItem<XBNode>, IParentItem<XBNode, XBNode>
    {
        private XBNode _parent;

        public XBNode()
        {
            Childs = new ChildItemCollection<XBNode, XBNode>(this);
        }

        XBNode IChildItem<XBNode>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        internal XBNode Parent => _parent;

        public ChildItemCollection<XBNode, XBNode> Childs { get; }

        internal virtual void Add(XBNode node)
        {
            if (node == this) throw new Exception("Recursial dependency not allowed");
            Childs.Add(node);
        }

        internal virtual void AddRange(IEnumerable<XBNode> nodes)
        {
            foreach (var node in nodes)
            {
                Add(node);
            }
        }

        internal virtual XBNode GetChild<T>()
        {
            foreach (var child in Childs)
            {
                if (child is T) return child;
            }

            return null;
        }

        internal virtual XBNode FirstParent<T>()
        {
            return _parent is T ? _parent : _parent?.FirstParent<T>();
        }

        internal virtual void Detach()
        {
            _parent?.Childs.Remove(this);
        }

        internal virtual void Attach(XBNode xbNode)
        {
            Detach();
            xbNode.Childs.Add(this);
        }
    }

    public class XBValue : XBNode
    {
        private readonly string _value;

        public XBValue(string value)
        {
            _value = value;
        }

        public String Value => _value;
    }

    public class XBWindow : XBNode
    {
        private int _width;
        private XBNode _widthNode;

        public XBWindow With()
        {
            return this;
        }


        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                _widthNode?.Detach();
            }
        }
    }

    public class XBFactory
    {
        public
    }
}