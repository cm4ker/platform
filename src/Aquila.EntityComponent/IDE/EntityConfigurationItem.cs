﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Avalonia.Controls;
using Aquila.EntityComponent.Configuration;
using Aquila.EntityComponent.Configuration.Editors;
using Aquila.Ide.Contracts;

namespace Aquila.EntityComponent.IDE
{
    public class EntityConfigurationItem : ConfigurationItemBase
    {
        private ObjectEditor _editor;
        private ObservableCollection<IConfigurationItem> _childs;

        public EntityConfigurationItem(ObjectEditor editor)
        {
            _editor = editor;
            _childs = new ObservableCollection<IConfigurationItem>();
            _childs.Add(new PropertyListConfigurationItem(_editor));
            _childs.Add(new ModuleListConfigurationItem(_editor));
            _childs.Add(new InterfaceListConfigurationItem(_editor));
            _childs.Add(new CommandListConfigurationItem(_editor));
        }

        public override string Caption
        {
            get => _editor.Name;
            set { }
        }

        public override bool IsEnable => true;

        public override ObservableCollection<IConfigurationItem> Childs => _childs;

        public override bool CanDelete => true;

        public override bool CanSearch => true;

        public override bool Search(string text)
        {
            return _editor.Name.Contains(text);
        }
    }
}