using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ZenPlatform.Controls.Avalonia;

namespace ZenPlatform.UIBuilder
{
    public class Invoice
    {
        public Guid Id { get; set; }

        public object ComplexProperty { get; set; }

        public int ComplexPropertyCurrentType => 1;

        public override string? ToString()
        {
            return "this is my invoice";
        }
    }

    public class MainWindow2 : Window
    {
        public MainWindow2()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            var picker = new ObjectPicker();
            var stackPanel = new StackPanel();
            var id = new ObjectPicker();
            
            
            stackPanel.Children.Add(picker);
            stackPanel.Children.Add(id);
            

            Content = stackPanel;
        }
    }

    public class DisplayedItem
    {
        public string Caption { get; set; }

        public string Value { get; set; }


        public override string? ToString()
        {
            return Caption;
        }
    }
}