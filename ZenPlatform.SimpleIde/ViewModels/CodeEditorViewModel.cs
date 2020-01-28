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
using ReactiveUI;

namespace ZenPlatform.SimpleIde.ViewModels
{
    [View(typeof(CodeEditorView))]
    public class CodeEditorViewModel : Document
    {
        private IConfigurationDocument _doc;
        public CodeEditorViewModel(IConfigurationDocument doc)
        {
            _doc = doc;


        }

        public TextDocument TextDocument => _doc.Text;

    }
}
