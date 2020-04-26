/*
namespace ZenPlatform.SimpleIde.Models
{
    public class ObjectConfigurationDocument : ReactiveObject, IConfigurationDocument
    {
        private IConfigurationItem _item;
        private bool _isChanged;
        public IObservable<bool> DocumentChanged { get; }

        public IObservable<IConfigurationItem> SaveDocument => SaveCommand;

        private Subject<IConfigurationItem> _save = new Subject<IConfigurationItem>();

        private ReactiveCommand<Unit, IConfigurationItem> SaveCommand;

        public ObjectConfigurationDocument(IConfigurationItem item)
        {
            Content = new TextDocument();
            
            _item = item;

            SaveCommand = ReactiveCommand.Create<Unit, IConfigurationItem>(d =>
            {
                _item.Context = XCHelper.DeserializeFromString(Content.Text);
                return _item;
            });


            Content.Text = FormatXML(_item.Context.SerializeToString());

            DocumentChanged = Observable.Merge(Content.WhenAnyValue(c => c.Text).Select(c => true), SaveCommand.Select(_ => false));
          
            
        }

    
        public void Save()
        {
            try
            {
                _item.Context = XCHelper.DeserializeFromString(Content.Text);
                IsChanged = false;
                _save.OnNext(_item);


            }
            catch (Exception ex)
            {
                _save.OnError(ex);
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

        public string Title => _item.Caption;
    }
}
*/

