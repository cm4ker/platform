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
using System.Reactive;

namespace ZenPlatform.SimpleIde.ViewModels
{
    [View(typeof(CodeEditorView))]
    public class CodeEditorViewModel : Document
    {
        private IConfigurationDocument _doc;
        public CodeEditorViewModel(IConfigurationDocument doc)
        {
            _doc = doc;

            _doc.WhenAnyValue(d => d.IsChanged).Subscribe(x => { Title = string.Format("{0}{1}",_doc.Title, x ?"*":""); });

            SaveCommand = ReactiveCommand.Create<Unit>(x => { _doc.Save(); });
        }

        public ReactiveCommand<Unit, Unit> SaveCommand;

        public bool IsChanged => _doc.IsChanged;
        
        public TextDocument TextDocument => _doc.Content;

    }
}
