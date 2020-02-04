using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Ide.Common.Editors
{
    class TypeSelectorViewModel: ReactiveObject
    {
        private ITypeManager _typeManager;
        private bool _result;
        public TypeSelectorViewModel(ITypeManager typeManager)
        {
            _typeManager = typeManager;
            DialogCommand = ReactiveCommand.Create<bool>(r =>
             {
                 DialogResult = r;
             });
          
        }

        public bool DialogResult
        {
            get => _result; 
            set => this.RaiseAndSetIfChanged(ref _result, value);
        }

        public IType SelectType { get; set; }

        public IEnumerable<IType> Types => _typeManager.Types;

        public ReactiveCommand<bool, Unit> DialogCommand;

    }
    public class UITypeSelector : Window
    {
        public static Interaction<Unit, IType> SelectType(ITypeManager typeManager)
        {
            var interop = new Interaction<Unit, IType>();

            interop.RegisterHandler(async interaction =>
            {
                var dialog = new UITypeSelector(typeManager);

                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    interaction.SetOutput(await dialog.ShowDialog<IType>(desktop.MainWindow));



            });
            return interop;
        }
        public UITypeSelector(ITypeManager typeManager)
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var view = new TypeSelectorViewModel(typeManager);
            this.DataContext = view;

            view.WhenAnyValue(v => v.DialogResult).Subscribe(r => {
                if (r)
                    Close(view.SelectType);
                else Close(null);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
