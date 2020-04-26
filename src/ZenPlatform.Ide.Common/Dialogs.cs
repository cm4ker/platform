using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Ide.Common.Editors;
using MessageBox.Avalonia;
using System.Threading.Tasks;
using MessageBox.Avalonia.Enums;

namespace ZenPlatform.Ide.Common
{
    public static class Dialogs
    {

        public static IObservable<IPType> SelectType(ITypeManager typeManager)
        {
            return SelectType(typeManager, t => true);
        }
        public static IObservable<IPType> SelectType(ITypeManager typeManager, Func<IPType, bool> filter )
        {
            /*
            var interop = new Interaction<Unit, IPType>();

            interop.RegisterHandler(async interaction =>
            {
                var view = new TypeSelectorViewModel(typeManager, filter);


                var dialog = new UITypeSelectorDialog();
                dialog.Model = view;

                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    interaction.SetOutput(await dialog.ShowDialog<IPType>(desktop.MainWindow));



            });
            return interop.Handle(Unit.Default);
            */

            var view = new TypeSelectorViewModel(typeManager, filter);


            var dialog = new UITypeSelectorDialog();
            dialog.Model = view;

            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return Observable.FromAsync(() => dialog.ShowDialog<IPType>(desktop.MainWindow), RxApp.MainThreadScheduler);

            return null;
        }

        public static IObservable<string> SelectText(string Caption, double TextBoxWidth = 300)
        {


            
            var dialog = new UITextBoxDialog(Caption, TextBoxWidth);

            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return Observable.FromAsync(() => dialog.ShowDialog<string>(desktop.MainWindow), RxApp.MainThreadScheduler);

            return null;
            
        }

        public static IObservable<string> OpenDirectory()
        {
            var dialog = new OpenFolderDialog();

            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return Observable.FromAsync(() => dialog.ShowAsync(desktop.MainWindow), RxApp.MainThreadScheduler);

            return null;
        }


        public static IObservable<ButtonResult> ShowSimpleDialog(string title, string text, ButtonEnum buttons)
        {
            
            var dialog = MessageBoxManager.GetMessageBoxStandardWindow(title, text, buttons);
            
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return Observable.FromAsync(() => dialog.ShowDialog(desktop.MainWindow), RxApp.MainThreadScheduler);

            return null;

        }

    }
}
