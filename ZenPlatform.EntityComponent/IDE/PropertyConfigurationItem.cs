using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.EntityComponent.IDE.Editors;
using ZenPlatform.Ide.Common;
using ZenPlatform.Ide.Contracts;


namespace ZenPlatform.EntityComponent.IDE
{
    [View(typeof(UiPropertyEditor))]
    class PropertyConfigurationItem : ReactiveObject, IConfigurationItem
    {
        private PropertyEditor _property;
        private ObjectEditor _objectEditor;
        private IType _selectedType;
        public PropertyConfigurationItem(PropertyEditor property, ObjectEditor objectEditor)
        {
            _property = property;
            _objectEditor = objectEditor;

            AddCommand = ReactiveCommand.CreateFromObservable(
                 () => Dialogs.SelectType(_objectEditor.Infrastructure.TypeManager, t=>!t.IsAbstract && !t.IsDto && Types.All(m=>m.Id!=t.Id)).Select(t=>t.GetMDType()));

            AddCommand.Subscribe(t => { _property.SetType(t); this.RaisePropertyChanged("Types"); });


            DeleteCommand = ReactiveCommand.Create(
                 () => SelectedType.GetMDType());

            DeleteCommand.Subscribe(t => { _property.UnsetType(t.Guid); this.RaisePropertyChanged("Types"); });

        }
        public string Caption
        {
            get => _property.Name;
            set
            {
                _property.Name = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsEnable => true;

        public bool IsExpanded { get; set; }

        public bool CanCreate => false;

        public bool CanDelete => true;

        public ObservableCollection<IConfigurationItem> Childs => null;

        public bool CanEdit => true;

        public IConfigurationItem Create(string name) => throw new NotImplementedException();

        public bool Search(string text)
        {
            return Caption.Contains(text);
        }

        public IEnumerable<IType> Types
        {
            get

            {
                return _property.Types.Join(_objectEditor.Infrastructure.TypeManager.Types.Where(s=>!s.IsAbstract), m => m.Guid,
                    t =>
                    {
                        if (t.IsTypeSpec) return t.BaseId;
                        return t.Id;
                    }
                    ,
                (m, t) => t); ;
            }
        }

        public ReactiveCommand<Unit, MDType> AddCommand { get; }
        public ReactiveCommand<Unit, MDType> DeleteCommand { get; }

        public IType SelectedType { get => _selectedType; set => this.RaiseAndSetIfChanged(ref _selectedType, value); }

        public bool CanSearch => true;
    }
}
