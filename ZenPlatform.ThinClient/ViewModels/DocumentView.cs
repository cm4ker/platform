using System.Reactive.Linq;
using Dock.Model.Controls;
using ReactiveUI;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.ThinClient.ViewModels
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

            // var result = Dialogs.GetOkCancel(_doc.Caption, "Save changed?");
            // result.Where(r => r).Subscribe(r=> { Save(); }) ;

            return true;// base.OnClose();
        }

        public void Save()
        {
            _doc.Save();
        }
    }
}
