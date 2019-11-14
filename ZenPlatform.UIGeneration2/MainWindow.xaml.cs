using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace ZenPlatform.UIBuilder
{
    public class VM : INotifyPropertyChanged
    {
        private string _input;
        private string _output;

        public string Input
        {
            get => _input;
            set
            {
                if (value == _input) return;
                _input = value;
                InputChanged();
                OnPropertyChanged();
            }
        }

        private void InputChanged()
        {
            Interpreter i = new Interpreter();
            Output = i.Run(_input);
        }


        public string Output
        {
            get => _output;
            set
            {
                if (value == _output) return;
                _output = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            DataContext = new VM();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Changed()
        {
        }
    }
}