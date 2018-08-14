using System.Globalization;
using System.IO;
using System.Xml;
using Avalonia.Controls;
using Portable.Xaml;
using ZenPlatform.Shared;
using ZenPlatform.Shared.Tree;
using ZenPlatform.UIBuilder.Interface;

namespace ZenPlatform.UIBuilder.Compilers
{
    public class AvaloniaXamlUICompiler
    {
        private XamlSchemaContext _context;
        private XamlXmlWriter _xamlWriter;

        public AvaloniaXamlUICompiler()
        {
            var x = new XamlSchemaContextSettings();
            _context = new XamlSchemaContext(null, null);
        }


        public string Compile(UINode node, StringWriter sw)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            using (XmlWriter xw = XmlWriter.Create(sw, settings))
            {
                using (_xamlWriter = new XamlXmlWriter(xw, _context, new XamlXmlWriterSettings()))
                {
                    VisitNode(node, sw);
                }
            }

            return sw.ToString();
        }


        protected virtual void VisitNode(UINode node, StringWriter sw)
        {
            ItemSwitch<Node>
                .Switch(node)
                .CaseIs<UIWindow>((i) => { VisitWindowNode(i, sw); })
                .CaseIs<UITextBox>(i => VisitTextBoxNode(i, sw))
                .CaseIs<UILabel>(i => VisitLabelNode(i, sw))
                .CaseIs<UIGroup>(i => VisitGroupNode(i, sw))
                .CaseIs<UITabControl>(i => VisitTabControlNode(i, sw))
                .CaseIs<UITab>(i => VisitTabNode(i, sw))
                .CaseIs<UICheckBox>(i => VisitCheckBoxNode(i, sw));

        }

        private void VisitCheckBoxNode(UICheckBox uiCheckBox, StringWriter sw)
        {
            XamlType tabType = new XamlType(typeof(CheckBox), _context);

            var contentProperty = new XamlMember(typeof(CheckBox).GetProperty("Content"), _context);

            _xamlWriter.WriteStartObject(tabType);

            //Content 
            _xamlWriter.WriteStartMember(contentProperty);
            _xamlWriter.WriteValue(uiCheckBox.Text.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            _xamlWriter.WriteEndObject();
        }

        private void VisitTabNode(UITab uiTab, StringWriter sw)
        {
            XamlType tabType = new XamlType(typeof(TabItem), _context);

            var textProperty = new XamlMember(typeof(TabItem).GetProperty("Header"), _context);
            var contentProperty = new XamlMember(typeof(TabItem).GetProperty("Content"), _context);


            _xamlWriter.WriteStartObject(tabType);

            //Text
            _xamlWriter.WriteStartMember(textProperty);
            _xamlWriter.WriteValue(uiTab.Header.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            //Items
            _xamlWriter.WriteStartMember(contentProperty);
            foreach (var node in uiTab.Childs)
            {
                VisitNode((UINode)node, sw);
            }
            _xamlWriter.WriteEndMember();

            _xamlWriter.WriteEndObject();
        }

        private void VisitTabControlNode(UITabControl uiTabControl, StringWriter sw)
        {
            XamlType textBoxType = new XamlType(typeof(TabControl), _context);

            var itemsProperty = new XamlMember(typeof(TabControl).GetProperty("Items"), _context);


            _xamlWriter.WriteStartObject(textBoxType);
            //Items
            _xamlWriter.WriteStartMember(itemsProperty);
            foreach (var node in uiTabControl.Childs)
            {
                VisitNode((UINode)node, sw);
            }
            _xamlWriter.WriteEndMember();

            _xamlWriter.WriteEndObject();
        }

        private void VisitLabelNode(UILabel uiLabel, StringWriter sw)
        {
            XamlType textBoxType = new XamlType(typeof(TextBlock), _context);

            var textProperty = new XamlMember(typeof(TextBlock).GetProperty("Text"), _context);

            _xamlWriter.WriteStartObject(textBoxType);

            //Text
            _xamlWriter.WriteStartMember(textProperty);
            _xamlWriter.WriteValue(uiLabel.Text.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            _xamlWriter.WriteEndObject();
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

            var heightProperty = new XamlMember(typeof(TextBox).GetProperty("Height"), _context);
            var widthProperty = new XamlMember(typeof(TextBox).GetProperty("Width"), _context);

            _xamlWriter.WriteStartObject(textBoxType);

            //Height
            _xamlWriter.WriteStartMember(heightProperty);
            _xamlWriter.WriteValue(uiTextBox.Height.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            //Width
            _xamlWriter.WriteStartMember(widthProperty);
            _xamlWriter.WriteValue(uiTextBox.Width.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

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
            _xamlWriter.WriteValue(uiWindow.Height.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            //Width
            _xamlWriter.WriteStartMember(widthProperty);
            _xamlWriter.WriteValue(uiWindow.Width.ToString(CultureInfo.InvariantCulture));
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