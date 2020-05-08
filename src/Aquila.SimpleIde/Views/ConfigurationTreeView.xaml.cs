using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Aquila.Ide.Common;
using Aquila.SimpleIde.ViewModels;

namespace Aquila.SimpleIde.Views
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
