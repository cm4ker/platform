using AvaloniaEdit.Document;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.SimpleIde.Models
{
    public class ObjectConfigurationDocument : ReactiveObject, IConfigurationDocument
    {
        private IConfiguratoinItem _item;
        private bool _isChanged;
        public ObjectConfigurationDocument(IConfiguratoinItem item)
        {
            Content = new TextDocument();
            
            _item = item;
           
            Content.Text = FormatXML(_item.Context.SerializeToString());
            IsChanged = false;
            Content.Changed += Text_Changed;
        }

        private void Text_Changed(object sender, DocumentChangeEventArgs e)
        {
            IsChanged = true;
        }
        public void Save()
        {
            try
            {
                _item.Context = XCHelper.DeserializeFromString(Content.Text);
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
        public bool IsChanged
        {
            get { return _isChanged; }
            set { this.RaiseAndSetIfChanged(ref _isChanged, value); }
        }
        public TextDocument Content { get; }

        public string Title => _item.Content;
    }
}
