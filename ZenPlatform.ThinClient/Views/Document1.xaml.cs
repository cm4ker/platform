﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZenPlatform.ThinClient.Views
{
    public class Document1 : UserControl
    {
        public Document1()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
