using Avalonia.Input;
using AvaloniaEdit.Document;
using Dock.Model.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ZenPlatform.Configuration.Structure;

using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using ZenPlatform.Ide.Contracts;
using ZenPlatform.Ide.Common;

namespace ZenPlatform.SimpleIde.ViewModels
{
    public class DocumentView : Document
    {
        private IConfigurationItem _doc;
        private readonly ObservableAsPropertyHelper<string> _title;
        public DocumentView(IConfigurationItem doc)
        {
            _doc = doc;
            this.Context = doc;
            

            _title = _doc.WhenAnyValue(d => d.IsChanged).Select(d=>string.Format("{0}{1}", _doc.Caption, d ? "*" : "")).ToProperty(this, vm => vm.Title);



           
        }
        public new string Title { get => _title.Value; set  { } }




        public override bool OnClose()
        {

            var result = Dialogs.GetOkCancel(_doc.Caption, "Save changed?");
            result.Where(r => r).Subscribe(r=> { Save(); }) ;

            return true;// base.OnClose();
        }

        public void Save()
        {
            _doc.Save();
        }
    }
}
