using Avalonia.Input;
using AvaloniaEdit.Document;
using Dock.Model.Controls;
using System;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Aquila.Ide.Contracts;
using Aquila.Ide.Common;
using Dock.Model;
using MessageBox.Avalonia.Enums;

namespace Aquila.SimpleIde.ViewModels
{
    public class DocumentView : Document
    {
        private IConfigurationItem _doc;
        private readonly ObservableAsPropertyHelper<string> _title;

        public DocumentView(IConfigurationItem doc)
        {
            _doc = doc;
            this.Context = doc;


            _title = _doc.WhenAnyValue(d => d.IsChanged)
                .Select(d => string.Format("{0}{1}", _doc.Caption, d ? "*" : "")).ToProperty(this, vm => vm.Title);
        }

        public new string Title
        {
            get => _title.Value;
            set { }
        }


        public override bool OnClose()
        {
            if (_doc.IsChanged)
            {
                Dialogs.ShowSimpleDialog(_doc.Caption, "Save changed?", ButtonEnum.YesNoCancel).Subscribe(r =>
                {
                    if (r == ButtonResult.Yes)
                    {
                        Save();

                        Factory.RemoveDockable(this, false);
                    }
                    else if (r == ButtonResult.No)
                    {
                        _doc.DiscardChange();
                        Factory.RemoveDockable(this, false);
                    }
                });

                return false;
            }
            else return true;
        }

        public void Save()
        {
            _doc.Save();
        }
    }
}