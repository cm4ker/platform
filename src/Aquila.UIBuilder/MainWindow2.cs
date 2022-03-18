using System;
using Avalonia;
using Avalonia.Controls;

namespace Aquila.UIBuilder
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
            var stackPanel = new StackPanel();

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