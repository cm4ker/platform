using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ZenPlatform.ThinClient.ViewModels;

namespace ZenPlatform.ThinClient.Views
{
    public class ConfigurationTreeView : UserControl
    {
        private readonly TreeView Tree;
        private readonly ConfigurationTreeViewModel model;

        public ConfigurationTreeView()
        {
            this.InitializeComponent();
            Tree = this.FindControl<TreeView>("TreeView");
            Tree.DoubleTapped += Tree_DoubleTapped;
        }

        private void Tree_DoubleTapped(object sender, RoutedEventArgs e)
        {
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}