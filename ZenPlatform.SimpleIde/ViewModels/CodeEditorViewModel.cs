using Avalonia.Input;
using AvaloniaEdit.Document;
using Dock.Model.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.SimpleIde.Models;
using ZenPlatform.SimpleIde.Views;

namespace ZenPlatform.SimpleIde.ViewModels
{
    [View(typeof(CodeEditorView))]
    public class CodeEditorViewModel : Document
    {
        private IConfiguratoinItem _item;
        public CodeEditorViewModel(IConfiguratoinItem item)
        {
            if (item.HasContext)
            {
                _item = item;
                TextDocument = CreateDocumentByType(item.Context);
                
            }
            else throw new ArgumentException("Item must has context", "item");

        }

        private TextDocument CreateDocumentByType(object obj)
        {
            switch (obj)
            {
               // case IConfiguratoinItem srt: return new TextDocument();
                default: return new TextDocument(FormatXML(obj.SerializeToString()));

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



        public TextDocument TextDocument { get; private set; }

    }
}
