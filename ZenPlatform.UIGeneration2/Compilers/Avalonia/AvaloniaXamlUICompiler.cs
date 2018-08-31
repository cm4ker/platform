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

            var headerProperty = dataGridColumnType.GetMember("Header");

            _xamlWriter.WriteStartObject(dataGridColumnType);

            //Header 
            _xamlWriter.WriteStartMember(headerProperty);
            _xamlWriter.WriteValue("Test");
            _xamlWriter.WriteEndMember();

            _xamlWriter.WriteEndObject();
        }

        private void VisitDataGridNode(UIDataGrid uiDataGrid, StringWriter sw)
        {
            XamlType dataGridType = _context.GetXamlType(typeof(DataGrid));

            var columnsProperty = dataGridType.GetMember("Columns");
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
            XamlType buttonType = _context.GetXamlType(typeof(Button));

            var contentProperty = buttonType.GetMember("Content");
            var commandProperty = buttonType.GetMember("Command");

            _xamlWriter.WriteStartObject(buttonType);

            //Content 
            _xamlWriter.WriteStartMember(contentProperty);
            _xamlWriter.WriteValue(uiButton.Text.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            //Command
            if (!string.IsNullOrEmpty(uiButton.OnClick))
            {
                _xamlWriter.WriteStartMember(commandProperty);
                //{{Binding {uiTextBox.DataSource}}}

                XamlType bindingXaml = _context.GetXamlType(typeof(Binding));

                XamlNodeList list = new XamlNodeList(_context);

                XamlMember pathProp = bindingXaml.GetMember("Path");

                _xamlWriter.WriteStartObject(bindingXaml);

                //Path
                _xamlWriter.WriteStartMember(pathProp);
                _xamlWriter.WriteValue(uiButton.OnClick);
                _xamlWriter.WriteEndMember();

                _xamlWriter.WriteEndObject();
                _xamlWriter.WriteEndMember();
            }

            _xamlWriter.WriteEndObject();
        }

        private void VisitCheckBoxNode(UICheckBox uiCheckBox, StringWriter sw)
        {
            XamlType checkBoxType = _context.GetXamlType(typeof(CheckBox));

            var contentProperty = checkBoxType.GetMember("Content");

            _xamlWriter.WriteStartObject(checkBoxType);

            //Content 
            _xamlWriter.WriteStartMember(contentProperty);
            _xamlWriter.WriteValue(uiCheckBox.Text.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            _xamlWriter.WriteEndObject();
        }

        private void VisitTabNode(UITab uiTab, StringWriter sw)
        {
            XamlType tabType = _context.GetXamlType(typeof(TabItem));

            var textProperty = tabType.GetMember("Header");
            var contentProperty = tabType.GetMember("Content");


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
            XamlType textBoxType = _context.GetXamlType(typeof(TabControl));

            var itemsProperty = textBoxType.GetMember("Items");


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
            XamlType textBoxType = _context.GetXamlType(typeof(TextBlock));

            var textProperty = textBoxType.GetMember("Text");

            _xamlWriter.WriteStartObject(textBoxType);

            //Text
            _xamlWriter.WriteStartMember(textProperty);
            _xamlWriter.WriteValue(uiLabel.Text.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            _xamlWriter.WriteEndObject();
        }

        private void VisitGroupNode(UIGroup uiGroup, StringWriter sw)
        {
            XamlType stackPanelType = _context.GetXamlType(typeof(StackPanel));

            //Props 
            var orientationProperty = stackPanelType.GetMember("Orientation");
            var contentProperty = stackPanelType.GetMember("Children");


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
            var ns = textBoxType.PreferredXamlNamespace;

            var heightProperty = textBoxType.GetMember("Height");//new XamlMember(typeof(TextBox).GetProperty("Height"), _context);
            var widthProperty = textBoxType.GetMember("Width");
            var textProperty = textBoxType.GetMember("Text");

            _xamlWriter.WriteStartObject(textBoxType);

            //Height
            _xamlWriter.WriteStartMember(heightProperty);
            _xamlWriter.WriteValue(uiTextBox.Height.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            //Width
            _xamlWriter.WriteStartMember(widthProperty);
            _xamlWriter.WriteValue(uiTextBox.Width.ToString(CultureInfo.InvariantCulture));
            _xamlWriter.WriteEndMember();

            //Text
            if (!string.IsNullOrEmpty(uiTextBox.DataSource))
            {
                _xamlWriter.WriteStartMember(textProperty);
                //{{Binding {uiTextBox.DataSource}}}

                XamlType bindingXaml = _context.GetXamlType(typeof(Binding));

                XamlNodeList list = new XamlNodeList(_context);



                XamlMember pathProp = bindingXaml.GetMember("Path");//new XamlMember(typeof(Binding).GetProperty("Path"), _context);
                XamlMember modeProp = bindingXaml.GetMember("Mode");
                _xamlWriter.WriteStartObject(bindingXaml);

                //Path
                _xamlWriter.WriteStartMember(pathProp);
                _xamlWriter.WriteValue(uiTextBox.DataSource);
                _xamlWriter.WriteEndMember();

                ////Mode 
                //_xamlWriter.WriteStartMember(modeProp);
                //_xamlWriter.WriteValue("TwoWay");
                //_xamlWriter.WriteEndMember();

                _xamlWriter.WriteEndObject();
                _xamlWriter.WriteEndMember();
            }

            _xamlWriter.WriteEndObject();
        }

        private void VisitWindowNode(UIWindow uiWindow, StringWriter sw)
        {
            XamlType windowType = _context.GetXamlType(typeof(Window));

            var heightProperty = windowType.GetMember("Height");
            var widthProperty = windowType.GetMember("Width");

            var contentProperty = windowType.GetMember("Content");

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