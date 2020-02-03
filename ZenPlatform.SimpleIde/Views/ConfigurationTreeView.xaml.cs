using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using ZenPlatform.SimpleIde.Models;
using ZenPlatform.SimpleIde.ViewModels;

namespace ZenPlatform.SimpleIde.Views
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
            if (Tree.SelectedItem is IConfiguratoinItem item && item.CanOpen)
            {

                ((ConfigurationTreeViewModel)Tree.DataContext).OpenItem = item;
            }


        }



        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
