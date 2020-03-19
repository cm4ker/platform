using System.Collections.ObjectModel;
using Dock.Model;
using Dock.Model.Controls;

namespace ZenPlatform.ThinClient.Dock
{
    public class ToolsDockContainer: ToolDock
    {
        public ToolsDockContainer()
        {
            this.VisibleDockables = new ObservableCollection<IDockable>();
            this.PinnedDockables = new ObservableCollection<IDockable>();
            this.HiddenDockables = new ObservableCollection<IDockable>();

            Proportion = double.NaN;
            Id = "Tools";
            Title = "Tools";
            IsCollapsable = false;
        }

        public void OpenTool(IDockable tool)
        {
            VisibleDockables.Add(tool);
            ActiveDockable = tool;
        }


    }
}
