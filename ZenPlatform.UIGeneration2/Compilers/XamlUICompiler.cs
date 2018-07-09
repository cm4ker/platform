using System.IO;
using Avalonia.Controls;
using Portable.Xaml;
using ZenPlatform.Shared;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.UIGeneration2
{
    public class XamlUICompiler
    {
        private XamlSchemaContext _context;
        private XamlXmlWriter _xamlWriter;

        public XamlUICompiler()
        {
            _context = new XamlSchemaContext(null, null);
        }



        public string Compile(UINode node, StringWriter sw)
        {
            using (_xamlWriter = new XamlXmlWriter(sw, _context, null))
                VisitNode(node, sw);
            return sw.ToString();
        }


        protected virtual void VisitNode(UINode node, StringWriter sw)
        {
            ItemSwitch<Node>
                .Switch(node)
                .CaseIs<UIWindow>((i) => { VisitWindowNode(i, sw); })
                .CaseIs<UITextBox>(i => VisitTextBoxNode(i, sw))
                .CaseIs<UIGroup>(i => VisitGroupNode(i, sw));
        }

        private void VisitGroupNode(UIGroup uiGroup, StringWriter sw)
        {
            XamlType stackPanelType = new XamlType(typeof(StackPanel), _context);

            //Props 
            var orientationProperty = new XamlMember(typeof(StackPanel).GetProperty("Orientation"), _context);
            var contentProperty = new XamlMember(typeof(StackPanel).GetProperty("Children"), _context);


            _xamlWriter.WriteStartObject(stackPanelType);

            //Orientation
            _xamlWriter.WriteStartMember(orientationProperty);
            _xamlWriter.WriteValue(uiGroup.Orientation == UIGroupOrientation.Horizontal ? "Horizontal" : "Vertical");
            _xamlWriter.WriteEndMember();

            //Content
            _xamlWriter.WriteStartMember(contentProperty);

            foreach (var node in uiGroup.Childs)
            {
                VisitNode((UINode)node, sw);
            }
            _xamlWriter.WriteEndMember();
            //EndContent

            _xamlWriter.WriteEndObject();
        }

        private void VisitTextBoxNode(UITextBox uiTextBox, StringWriter sw)
        {
            XamlType textBoxType = new XamlType(typeof(TextBox), _context);
            _xamlWriter.WriteStartObject(textBoxType);
            _xamlWriter.WriteEndObject();
        }

        private void VisitWindowNode(UIWindow uiWindow, StringWriter sw)
        {
            XamlType windowType = new XamlType(typeof(Window), _context);

            var heightProperty = new XamlMember(typeof(Window).GetProperty("Height"), _context);
            var widthProperty = new XamlMember(typeof(Window).GetProperty("Width"), _context);

            var contentProperty = new XamlMember(typeof(Window).GetProperty("Content"), _context);

            _xamlWriter.WriteStartObject(windowType);

            //Height
            _xamlWriter.WriteStartMember(heightProperty);
            _xamlWriter.WriteValue(uiWindow.Height.ToString());
            _xamlWriter.WriteEndMember();

            //Width
            _xamlWriter.WriteStartMember(widthProperty);
            _xamlWriter.WriteValue(uiWindow.Width.ToString());
            _xamlWriter.WriteEndMember();

            //Content
            _xamlWriter.WriteStartMember(contentProperty);

            foreach (var node in uiWindow.Childs)
            {
                VisitNode((UINode)node, sw);
            }

            _xamlWriter.WriteEndMember();
            _xamlWriter.WriteEndObject();
        }
    }
}