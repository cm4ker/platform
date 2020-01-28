using AvaloniaEdit.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.SimpleIde.Models
{
    public class ObjectConfigurationDocument : IConfigurationDocument
    {
        private IConfiguratoinItem _item;
        public ObjectConfigurationDocument(IConfiguratoinItem item)
        {
            Text = new TextDocument();
            Text.Changed += Text_Changed;
            _item = item;
            Text.Text = FormatXML(_item.Context.SerializeToString());
        }

        private void Text_Changed(object sender, DocumentChangeEventArgs e)
        {
            IsChanged = true;
        }
        public void Save()
        {
            try
            {
                _item.Context = XCHelper.DeserializeFromString(Text.Text);
                IsChanged = false;
            }
            catch (Exception ex)
            {

            }
        }

        private string FormatXML(string xml)
        {
            string result = "";

            using (var mStream = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(mStream, Encoding.Unicode))
                {
                    XmlDocument document = new XmlDocument();

                    try
                    {
                        // Load the XmlDocument with the XML.
                        document.LoadXml(xml);

                        writer.Formatting = Formatting.Indented;

                        // Write the XML into a formatting XmlTextWriter
                        document.WriteContentTo(writer);
                        writer.Flush();
                        mStream.Flush();

                        // Have to rewind the MemoryStream in order to read
                        // its contents.
                        mStream.Position = 0;

                        // Read MemoryStream contents into a StreamReader.
                        using (StreamReader sReader = new StreamReader(mStream))
                        {

                            // Extract the text from the StreamReader.
                            string formattedXml = sReader.ReadToEnd();

                            result = formattedXml;
                        }
                    }
                    catch (XmlException)
                    {
                        // Handle the exception
                    }
                }
            }

            return result;
        }
        public bool IsChanged { get; private set; }
        public TextDocument Text { get; }
    }
}
