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
        private string _output1;
        private string _output2;

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
            var o = i.Run(_input);
            Output1 = o.Output1;
            Output2 = o.Output2;
        }


        public string Output1
        {
            get => _output1;
            set
            {
                if (value == _output1) return;
                _output1 = value;
                OnPropertyChanged();
            }
        }

        public string Output2
        {
            get => _output2;
            set
            {
                if (value == _output2) return;
                _output2 = value;
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