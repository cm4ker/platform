using System;
using System.Collections.ObjectModel;
using ReactiveUI;
using ZenPlatform.ThinClient.ViewModels;

namespace ZenPlatform.ThinClient.Models
{
    /// <summary>
    /// Элемент конфигурации
    /// </summary>
    public class ConfItemModel : ViewModelBase
    {
        public ConfItemModel()
        {
            Childs = new ObservableCollection<ConfItemModel>();
        }

        private Guid _itemId;
        private string _name;

        public Guid ItemId
        {
            get => _itemId;
            set => this.RaiseAndSetIfChanged(ref _itemId, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }


        public ObservableCollection<ConfItemModel> Childs { get; set; }
    }
}