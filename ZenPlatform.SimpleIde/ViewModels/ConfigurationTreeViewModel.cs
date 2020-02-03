using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Dock.Model.Controls;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.SimpleIde.Models;
using ReactiveUI;
using ZenPlatform.SimpleIde.Views;
using System.Collections.ObjectModel;
using DynamicData.Binding;
using DynamicData;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace ZenPlatform.SimpleIde.ViewModels
{

    [View(typeof(ConfigurationTreeView))]
    public class ConfigurationTreeViewModel: Tool
    {

        private ICollection<IConfiguratoinItem> _openedConfiguration;

        private ObservableAsPropertyHelper<IEnumerable<IConfiguratoinItem>> _configuration;
        private string _searchQuery;
        private IConfiguratoinItem _openItem;

        public ConfigurationTreeViewModel()
        {

            Observable.Empty<IConfiguratoinItem>();
            _openedConfiguration = new ObservableCollection<IConfiguratoinItem>()
            {
                new SimpleConfigurationItem("test", new MDRoot()),
                 new SimpleConfigurationItem("uuu", new MDRoot()),
                 new SimpleConfigurationItem("ee", new MDRoot()),
                 new SimpleConfigurationItem("tewwst", new MDRoot()),
                 new SimpleConfigurationItem("rgg", new MDRoot()),
                 new SimpleConfigurationItem("uu", new MDRoot()),
            };

             Search = ReactiveCommand.CreateFromObservable<string, IEnumerable<IConfiguratoinItem>>(
                (query) =>
                {
                    return Observable.Start(() => _openedConfiguration.Where(i => i.Content.Contains(query)));
                },
                this.WhenAnyValue(vm=>vm.SearchQuery).Select(q=>!string.IsNullOrEmpty(q))
            );

            
            //_configuration = Observable.Merge(Search, Search.IsExecuting.Where(e => e).Select(_ => _openedConfiguration)).ToProperty(this, vm => vm.Configuration);

            _configuration = Observable.Merge(Search, this.WhenAnyValue(vm => vm.SearchQuery)
                .Where(q => string.IsNullOrEmpty(q)).Select(_ => _openedConfiguration)).ToProperty(this, vm => vm.Configuration);
            //_configuration = Search.ToProperty(this, vm => vm.Configuration);

            this.WhenAnyValue(vm => vm.SearchQuery).Throttle(TimeSpan.FromSeconds(0.5)).InvokeCommand(Search);
        }


        public IConfiguratoinItem OpenItem
        {
            get => _openItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _openItem, value);
            }
        }
        

        public event EventHandler<IConfiguratoinItem> OnOpenItem;
        public IEnumerable<IConfiguratoinItem> Configuration => _configuration.Value;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                this.RaiseAndSetIfChanged(ref _searchQuery, value);
            }
        }
        public ReactiveCommand<string, IEnumerable<IConfiguratoinItem>> Search { get; }

        public ReactiveCommand<Unit, IConfiguratoinItem> Open { get; }


        private async Task<IEnumerable<IConfiguratoinItem>> SearchAsync(string query, CancellationToken token)
        {

            return await Task.Run(()=> _openedConfiguration.Where(i=>i.Content.Contains(query)));
        }

    }
}
