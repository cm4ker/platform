using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ZenPlatform.UIBuilder
{
    public class VM : INotifyPropertyChanged
    {
        private string _input;
        private string _input2;
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


        public string Input2
        {
            get => _input2;
            set
            {
                if (value == _input2) return;
                _input2 = value;
                InputChanged2();
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

        private void InputChanged2()
        {
            Interpreter i = new Interpreter();
            var o = i.RunQuery(_input2);
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
}