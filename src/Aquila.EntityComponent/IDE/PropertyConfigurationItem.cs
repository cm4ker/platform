using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Aquila.Configuration.Common;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.EntityComponent.Configuration;
using Aquila.EntityComponent.Configuration.Editors;
using Aquila.EntityComponent.IDE.Editors;
using Aquila.Ide.Common;
using Aquila.Ide.Contracts;


namespace Aquila.EntityComponent.IDE
{
    [View(typeof(UiPropertyEditor))]
    class PropertyConfigurationItem : ConfigurationItemBase
    {
        private PropertyEditor _property;
        private ObjectEditor _objectEditor;
        private IPType _selectedType;
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
        public override string Caption
        {
            get => _property.Name;
            set
            {
                _property.Name = value;
                this.RaisePropertyChanged();
            }
        }


        public override bool CanDelete => true;


        public override bool CanEdit => true;


        public override bool Search(string text)
        {
            return Caption.Contains(text);
        }

        public IEnumerable<IPType> Types
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

        public IPType SelectedType { get => _selectedType; set => this.RaiseAndSetIfChanged(ref _selectedType, value); }

        public override bool CanSearch => true;

    }
}
