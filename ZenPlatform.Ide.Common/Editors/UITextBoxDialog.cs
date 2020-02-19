using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Ide.Common.Editors
{
    public class UITextBoxDialog: Window
    {
        private TextBox _textBox;
        public UITextBoxDialog(string Caption, double TextBoxWidth = 200)
        {
            this.SizeToContent = SizeToContent.WidthAndHeight;


            var buttonPanel = new StackPanel()
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
            };

            var ok = new Button()
            {
                Content = "Ok",
                Margin = new Thickness(5),
                Tag = true
            };
            ok.Click += Click;
            var cancel = new Button()
            {
                Content = "Cancel",
                Margin = new Thickness(5),
                Tag = false
            };
            cancel.Click += Click;


            buttonPanel.Children.Add(ok);
            buttonPanel.Children.Add(cancel);

            var mainPanel = new StackPanel()
            {
                Orientation = Avalonia.Layout.Orientation.Vertical,
                Margin = new Thickness(5)
            };
            mainPanel.Children.Add(new TextBlock()
            {
                Text = Caption
            });
            _textBox = new TextBox()
            {
                Width = TextBoxWidth
            };

            mainPanel.Children.Add(_textBox);
            mainPanel.Children.Add(buttonPanel);

            this.Title = Caption;
            this.Content = mainPanel;
        }

        private void Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag is bool res)
                {
                    e.Handled = true;
                    if (res)
                        Close(_textBox.Text);
                    else Close(null);

                }
            }
            
        }
    }
}
