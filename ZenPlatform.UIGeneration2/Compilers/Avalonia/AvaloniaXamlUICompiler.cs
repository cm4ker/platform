using System.Globalization;
using System.IO;
using System.Xml;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Context;
using Portable.Xaml;
using Portable.Xaml.Schema;
using ZenPlatform.Controls.Avalonia;
using ZenPlatform.Shared;
using ZenPlatform.Shared.Tree;
using ZenPlatform.UIBuilder.Compilers.Avalonia;
using ZenPlatform.UIBuilder.Interface;
using ZenPlatform.UIBuilder.Interface.DataGrid;

namespace ZenPlatform.UIBuilder.Compilers
{

    public class AvaloniaXamlUICompiler
    {
        private XamlSchemaContext _context;
        private XamlXmlWriter _xamlWriter;

        public AvaloniaXamlUICompiler()
        {
            var x = new XamlSchemaContextSettings();
            _context = new AvaloniaCustomXamlSchemaContext(new AvaloniaRuntimeTypeProvider());
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
                .CaseIs<UICheckBox>(i => VisitCheckBoxNode(i, sw))
                .CaseIs<UIButton>(i => VisitButtonNode(i, sw))
                .CaseIs<UIDataGrid>(i => VisitDataGridNode(i, sw))
                .CaseIs<UIDataGridColumn>(i => VisitDataGridColumnNode(i, sw))
                .CaseIs<UIObjectPicker>(i => VisitObjectPicker(i, sw));
        }

        private void VisitObjectPicker(UIObjectPicker uiObjectPicker, StringWriter sw)
        {
            XamlType objectPickerType = new XamlType(typeof(ObjectPicker), _context);
            _xamlWriter.WriteStartObject(objectPickerType);
            _xamlWriter.WriteEndObject();
        }

        private void VisitDataGridColumnNode(UIDataGridColumn uiDataGridColumn, StringWriter sw)
        {
            XamlType dataGridColumnType = _context.GetXamlType(typeof(DataGridTextColumn));

            var headerProperty = new XamlMember("Header", dataGridColumnType, false);

            _xamlWriter.WriteStartObject(dataGridColumnType);

            //Header 
            _xamlWriter.WriteStartMember(headerProperty);
            _xamlWriter.WriteValue("Test");
            _xamlWriter.WriteEndMember();

            _xamlWriter.WriteEndObject();
        }

        private void VisitDataGridNode(UIDataGrid uiDataGrid, StringWriter sw)
        {
            XamlType dataGridType = new XamlType(typeof(DataGrid), _context);

            var columnsProperty = new XamlMember(typeof(DataGrid).GetProperty("Columns"), _context);
            _xamlWriter.WriteStartObject(dataGridType);

            //Items
            _xamlWriter.WriteStartMember(columnsProperty);
            foreach (var node in uiDataGrid.Childs)
            {
                VisitNode((UINode)node, sw);
            }

            _xamlWriter.WriteEndMember();

            _xamlWriter.WriteEndObject();
        }

        private void VisitButtonNode(UIButton uiButton, StringWriter sw)
        {
            XamlType buttonType = new XamlType(typeof(Button), _context);

            var contentProperty = new XamlMember(typeof(Button).GetProperty("Content"), _context);

            _xamlWriter.WriteStartObject(buttonType);

            //Content 
            _xamlWriter.WriteStartMember(contentProperty);
            _xamlWriter.WriteValue(uiButton.Text.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            _xamlWriter.WriteEndObject();
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
            XamlType textBoxType = _context.GetXamlType(typeof(TextBox));

            var heightProperty = textBoxType.GetMember("Height");//new XamlMember(typeof(TextBox).GetProperty("Height"), _context);
            var widthProperty = new XamlMember(typeof(TextBox).GetProperty("Width"), _context);
            var textProperty = new XamlMember(typeof(TextBox).GetProperty("Text"), _context);




            _xamlWriter.WriteStartObject(textBoxType);

            //Height
            _xamlWriter.WriteStartMember(heightProperty);
            _xamlWriter.WriteValue(uiTextBox.Height.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            //Width
            _xamlWriter.WriteStartMember(widthProperty);
            _xamlWriter.WriteValue(uiTextBox.Width.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            ////Text
            //if (!string.IsNullOrEmpty(uiTextBox.DataSource))
            //{
            //    _xamlWriter.WriteStartMember(textProperty);
            //    //{{Binding {uiTextBox.DataSource}}}

            //    XamlType bindingXaml = _context.GetXamlType(typeof(Binding));
            //    XamlMember pathProp = bindingXaml.GetMember("Path");//new XamlMember(typeof(Binding).GetProperty("Path"), _context);
            //    _xamlWriter.WriteStartObject(bindingXaml);
            //    _xamlWriter.WriteStartMember(pathProp);
            //    _xamlWriter.WriteValue(uiTextBox.DataSource);
            //    _xamlWriter.WriteEndMember();
            //    _xamlWriter.WriteEndObject();
            //    _xamlWriter.WriteEndMember();
            //}

            _xamlWriter.WriteEndObject();
        }

        private void VisitWindowNode(UIWindow uiWindow, StringWriter sw)
        {
            XamlType windowType = _context.GetXamlType(typeof(Window));

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