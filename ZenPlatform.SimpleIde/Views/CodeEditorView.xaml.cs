using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using System.IO;
using System.Xml;

namespace ZenPlatform.SimpleIde.Views
{

    public class CodeEditorView : UserControl
    {
        private readonly TextEditor _textEditor;
        public CodeEditorView()
        {
            this.InitializeComponent();


            _textEditor = this.FindControl<TextEditor>("Editor");

            

            
            /*
            using (Stream s = new FileStream("CSharp-Mode-Dark.xshd", FileMode.Open))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    _textEditor.SyntaxHighlighting = _textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            */
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
