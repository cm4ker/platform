using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Aquila.Data;
using Aquila.Metadata;
using Aquila.Runtime;
using Aquila.Runtime.Querying;
using JetBrains.Annotations;

namespace Aquila.UIBuilder
{
    public class VM : INotifyPropertyChanged
    {
        private string _input;
        private string _input2;
        private string _output1;
        private string _output2;

        private string _connectionString;
        private DatabaseRuntimeContext _drContext;
        private DataConnectionContext _dcContext;

        public VM()
        {
            _connectionString = TestMetadata.DefaultConnetionString;
            _dcContext = new DataConnectionContext(SqlDatabaseType.SqlServer, _connectionString);
            _drContext = new DatabaseRuntimeContext();
            _drContext.Load(_dcContext);
        }

        public string Input
        {
            get => _input;
            set
            {
                if (value == _input) return;
                _input = value;
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
        }

        private void InputChanged2()
        {
            Interpreter i = new Interpreter(_drContext);
            var o = i.RunQuery(_input2);
            Output1 = o.Output1;
            Output2 = o.Output2;

            try
            {
                var crud = new CRUDQueryGenerator();
                var md = _drContext.GetMetadata();
                var invoice = md.GetSemanticByName("Invoice");

                var insert = CRUDQueryGenerator.GetSaveInsert(invoice, _drContext);
                var update = CRUDQueryGenerator.GetSaveUpdate(invoice, _drContext);
                var select = CRUDQueryGenerator.GetLoad(invoice, _drContext);

                Input = $"{insert}\n{update}\n{select}";
            }
            catch (Exception ex)
            {
                Input = ex.ToString();
            }
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