using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZenPlatform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var picker = new Field() {Value = new Invoice(), IsTypeButtonVisible = false};
            var stackPanel = new StackPanel();
            var id = new Field() {Value = Guid.NewGuid()};

            stackPanel.Children.Add(picker);
            stackPanel.Children.Add(id);


            Content = stackPanel;
        }
    }
}