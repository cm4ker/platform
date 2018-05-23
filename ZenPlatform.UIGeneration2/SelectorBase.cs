using System;
using System.Collections.Generic;

namespace ZenPlatform.UIGeneration2 {
    [Serializable]
    public abstract class SelectorBase : ObservableObject {

        IEnumerable<String> _classPropertyNames;
        String _displayMemberPath = String.Empty;
        String _editorBindingTargetProperty = String.Empty;
        Boolean _hasComplexSelectedItem;
        String _itemsSourceBindingPath = String.Empty;
        EnumerablePropertyItem _selectedEnumerablePropertyItem;
        String _selectedItemPathText = String.Empty;
        String _selectedValueBindingPath = String.Empty;
        String _selectedValuePath = String.Empty;

        public IEnumerable<String> ClassPropertyNames {
            get { return _classPropertyNames; }
            set {
                _classPropertyNames = value;
                RaisePropertyChanged();
            }
        }

        public String DisplayMemberPath {
            get { return _displayMemberPath; }
            set {
                _displayMemberPath = value;
                RaisePropertyChanged();
            }
        }

        public String DisplayMemberPathText {
            get {
                if (String.IsNullOrWhiteSpace(this.DisplayMemberPath)) {
                    return String.Empty;
                }
                return $"DisplayMemberPath=\"{this.DisplayMemberPath}\" ";
            }
        }

        public String EditorBindingTargetProperty {
            get { return _editorBindingTargetProperty; }
            set {
                _editorBindingTargetProperty = value;
                RaisePropertyChanged();
            }
        }

        public Boolean HasComplexSelectedItem {
            get { return _hasComplexSelectedItem; }
            set {
                _hasComplexSelectedItem = value;
                RaisePropertyChanged();
            }
        }

        public String ItemsSourceBindingPath {
            get { return _itemsSourceBindingPath; }
            set {
                _itemsSourceBindingPath = value;
                RaisePropertyChanged();
            }
        }

        public String ItemsSourceText {
            get {
                if (String.IsNullOrWhiteSpace(this.ItemsSourceBindingPath)) {
                    return String.Empty;
                }
                return $"ItemsSource=\"{{Binding Path={this.ItemsSourceBindingPath}}}\" ";
            }
        }

        public EnumerablePropertyItem SelectedEnumerablePropertyItem {
            get { return _selectedEnumerablePropertyItem; }
            set {
                _selectedEnumerablePropertyItem = value;
                RaisePropertyChanged();
                AfterSelectedEnumerablePropertyItem();
            }
        }

        public String SelectedItemPathText {
            get { return _selectedItemPathText; }
            set {
                _selectedItemPathText = value;
                RaisePropertyChanged();
            }
        }

        public String SelectedValueBindingPath {
            get { return _selectedValueBindingPath; }
            set {
                _selectedValueBindingPath = value;
                RaisePropertyChanged();
            }
        }

        public String SelectedValuePath {
            get { return _selectedValuePath; }
            set {
                _selectedValuePath = value;
                RaisePropertyChanged();
                if (String.IsNullOrWhiteSpace(_selectedValuePath)) {
                    this.EditorBindingTargetProperty = "SelectedItem";
                } else {
                    this.EditorBindingTargetProperty = "SelectedValue";
                }
            }
        }

        public String SelectedValuePathText {
            get {
                if (String.IsNullOrWhiteSpace(this.SelectedValuePath)) {
                    return String.Empty;
                }
                return $"SelectedValuePath=\"{this.SelectedValuePath}\" ";
            }
        }

        void AfterSelectedEnumerablePropertyItem() {
            if (this.SelectedEnumerablePropertyItem == null || !this.SelectedEnumerablePropertyItem.IsClass) {
                this.HasComplexSelectedItem = false;
                this.ClassPropertyNames = null;
                this.DisplayMemberPath = String.Empty;
                this.SelectedValueBindingPath = String.Empty;
            } else {
                this.HasComplexSelectedItem = true;
                this.ClassPropertyNames = this.SelectedEnumerablePropertyItem.ClassPropertyNames;
            }

            if (this.SelectedEnumerablePropertyItem == null) {
                this.EditorBindingTargetProperty = String.Empty;
                this.ItemsSourceBindingPath = String.Empty;
            } else {
                this.ItemsSourceBindingPath = this.SelectedEnumerablePropertyItem.Name;
                this.EditorBindingTargetProperty = "SelectedItem";
            }
        }

    }
}
