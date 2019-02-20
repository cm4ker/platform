using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using ZenPlatform.IdeIntegration.Client.Models;

namespace ZenPlatform.IdeIntegration.Client.ViewModel.Configuration
{
    public class ConfTreeViewModel : ReactiveObject
    {
        /// <summary>
        /// Модель дерева конфигурации.
        /// </summary>
        public ConfTreeViewModel()
        {
            Items = new ObservableCollection<ConfItemModel>();
        }


        /// <summary>
        /// Элементы дерева
        /// </summary>
        public ObservableCollection<ConfItemModel> Items { get; }
    }
}