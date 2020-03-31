using System.Collections.ObjectModel;
using Dock.Model;
using Dock.Model.Controls;
using ZenPlatform.ClientRuntime.ViewModels;

namespace ZenPlatform.ClientRuntime.Dock
{
    public class DocumentDockContainer: DocumentDock
    {
        public DocumentDockContainer()
        {
            this.VisibleDockables = new ObservableCollection<IDockable>();
            this.PinnedDockables = new ObservableCollection<IDockable>();
            this.HiddenDockables = new ObservableCollection<IDockable>();

            Id = "DocumentsPane";
            Title = "DocumentsPane";
            IsCollapsable = false;
            Proportion = double.NaN;
        }


        public void OpenDocument(DocumentView document)
        {
            VisibleDockables.Add(document);
            ActiveDockable = document;
        }
    }
}
