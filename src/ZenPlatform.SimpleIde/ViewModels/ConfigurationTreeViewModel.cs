using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Dock.Model.Controls;
using ZenPlatform.Configuration.Structure;
using ReactiveUI;
using ZenPlatform.SimpleIde.Views;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using ZenPlatform.Ide.Common;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.SimpleIde.ViewModels
{

    [View(typeof(ConfigurationTreeView))]
    public class ConfigurationTreeViewModel: Tool
    {

        private ICollection<IConfigurationItem> _openedConfiguration;

        private ObservableAsPropertyHelper<IEnumerable<IConfigurationItem>> _configuration;
        private string _searchQuery;

        private IConfigurationItem _selectedItem;

        public void Open(Project project)
        {

               _openedConfiguration
                = new ObservableCollection<IConfigurationItem>(project.Editors.Select(e => e.GetConfigurationTree()));
        }

        public ConfigurationTreeViewModel(Project project)
        {

            _openedConfiguration = new ObservableCollection<IConfigurationItem>(project.Editors.Select(e => e.GetConfigurationTree()));


            Search = ReactiveCommand.Create<string, IEnumerable<IConfigurationItem>>(
                (query) =>
                {
                    return  _openedConfiguration.Where(i => i.CanSearch && i.Search(query));
                },
                this.WhenAnyValue(vm=>vm.SearchQuery).Select(q=>!string.IsNullOrEmpty(q))
            );

            
            //_configuration = Observable.Merge(Search, Search.IsExecuting.Where(e => e).Select(_ => _openedConfiguration)).ToProperty(this, vm => vm.Configuration);

            _configuration = Observable.Merge(Search, this.WhenAnyValue(vm => vm.SearchQuery)
                .Where(q => string.IsNullOrEmpty(q)).Select(_ => _openedConfiguration)).ToProperty(this, vm => vm.Configuration);
            //_configuration = Search.ToProperty(this, vm => vm.Configuration);

            this.WhenAnyValue(vm => vm.SearchQuery).Throttle(TimeSpan.FromSeconds(0.5)).InvokeCommand(Search);


            EditItem = ReactiveCommand.Create(() => SelectedItem);

            CreateItem = ReactiveCommand.CreateFromObservable(() =>
              Dialogs.SelectText("Create new:").Where(s => !string.IsNullOrEmpty(s)).Select(name => SelectedItem.Create(name))
            );

        }

        public IConfigurationItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedItem, value);
            }
        }

        public IEnumerable<IConfigurationItem> Configuration => _configuration.Value;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                this.RaiseAndSetIfChanged(ref _searchQuery, value);
            }
        }
        public ReactiveCommand<string, IEnumerable<IConfigurationItem>> Search { get; }

        public ReactiveCommand<Unit, IConfigurationItem> EditItem { get; }

        public ReactiveCommand<Unit, IConfigurationItem> CreateItem { get; }

    }
}
