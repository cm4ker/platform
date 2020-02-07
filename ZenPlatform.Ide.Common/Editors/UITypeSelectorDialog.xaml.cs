using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Ide.Common.Editors
{
    public class TypeSelectorViewModel: ReactiveObject
    {
        private ITypeManager _typeManager;
        private bool _result;
        private IPType _selectType;
        private Func<IPType, bool> _filter;
        public TypeSelectorViewModel(ITypeManager typeManager, Func<IPType, bool> filter)
        {
            _typeManager = typeManager;
            _filter = filter;
            OkDialogCommand = ReactiveCommand.Create<Unit, bool>(b =>
             {
                 return true;
             }, this.WhenAnyValue(v=>v.SelectType).Select(t => !(t is null)));

            CancelDialogCommand = ReactiveCommand.Create<Unit, bool>(b=>
            {
                return false;
            }); 
        }

        public bool DialogResult
        {
            get => _result; 
            set => this.RaiseAndSetIfChanged(ref _result, value);
        }

        public IPType SelectType
        {
            get => _selectType;
            set => this.RaiseAndSetIfChanged(ref _selectType, value);
        }

        public IEnumerable<IPType> Types => _typeManager.Types.Where(_filter);

        public ReactiveCommand<Unit, bool> OkDialogCommand { get; set; }
        public ReactiveCommand<Unit, bool> CancelDialogCommand { get; set; }

    }
    public class UITypeSelectorDialog : Window
    {

        public UITypeSelectorDialog()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }
        private TypeSelectorViewModel _model;
        public TypeSelectorViewModel Model { get => _model;
            set
            {
                _model = value;
                this.DataContext = value;

                _model.OkDialogCommand.Subscribe(b => Close(_model.SelectType));
                _model.CancelDialogCommand.Subscribe(b => Close(null));

            }
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
