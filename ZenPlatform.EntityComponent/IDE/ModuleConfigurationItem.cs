﻿using AvaloniaEdit.Document;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Ide.Common;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.EntityComponent.IDE
{ 
    [View(typeof(CodeEditorView))]
    public class ModuleConfigurationItem : ConfigurationItemBase
    {
        private readonly ModuelEditor _editor;
        private ObservableAsPropertyHelper<bool> _isChanged;
        
        public ModuleConfigurationItem(ModuelEditor editor)
        {
            _editor = editor;
            Document = new TextDocument(_editor.ModuleText);
            
           _isChanged = Document.WhenAnyValue(doc => doc.Text).Select(t => true).ToProperty(this, vm => vm.IsChanged);

        }

  

        public override string Caption { get => _editor.ModuleName; set { } }

        public override bool CanEdit => true;

        public override bool CanDelete => true;


        public override void Save()
        {
            _editor.ModuleText = Document.Text;
            
        }

        public TextDocument Document { get; set; }

        public override bool CanSave => true;
        public override bool IsChanged => _isChanged.Value;// _isChanged.Value;

        public string Title => Caption;

    }
}
