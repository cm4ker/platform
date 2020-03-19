using Dock.Model.Controls;

namespace ZenPlatform.ThinClient.ViewModels
{
    public class DocumentView : Document
    {
        private string _title;

        public DocumentView()
        {
            _title = "Title";
        }

        public new string Title
        {
            get => _title;
            set { }
        }


        public override bool OnClose()
        {
            // var result = Dialogs.GetOkCancel(_doc.Caption, "Save changed?");
            // result.Where(r => r).Subscribe(r=> { Save(); }) ;

            return true; // base.OnClose();
        }
    }
}