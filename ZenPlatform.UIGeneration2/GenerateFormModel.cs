using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ZenPlatform.UIGeneration2 {
    public class GenerateFormModel : ObservableObject {

        Boolean _addNameToUIControls;
        RootObject _columnRootObject;
        String _formHeader;
        Boolean _isLabelPositionEnabled = true;
        Boolean _isWrapGeneratedCodeInBorderEnabled;
        LabelPosition _labelPosition = LabelPosition.None;
        RootObject _rootObject;
        Boolean _showTableSectionTitle;
        Boolean _wrapGeneratedCodeInBorder;

        public Boolean AddNameToUIControls {
            get { return _addNameToUIControls; }
            set {
                _addNameToUIControls = value;
                RaisePropertyChanged();
            }
        }

        public IList<IList<PropertyInformationViewModel>> ColumnLayouts { get; } = new List<IList<PropertyInformationViewModel>>();

        public RootObject ColumnRootObject {
            get { return _columnRootObject; }
            set {
                _columnRootObject = value;
                RaisePropertyChanged();
                this.ShowTableSectionTitle = _columnRootObject == RootObject.TableView;
                ConfigureLabelPosition();
            }
        }

        public CreateObjectDefinition CreateObjectDefinition { get; set; }

        public String FormHeader {
            get { return _formHeader; }
            set {
                _formHeader = value;
                RaisePropertyChanged();
            }
        }

        public Boolean HideRootLayoutOptions { get; }

        public Boolean IsLabelPositionEnabled {
            get { return _isLabelPositionEnabled; }
            set {
                _isLabelPositionEnabled = value;
                RaisePropertyChanged();
            }
        }

        public Boolean IsWrapGeneratedCodeInBorderEnabled {
            get { return _isWrapGeneratedCodeInBorderEnabled; }
            set {
                _isWrapGeneratedCodeInBorderEnabled = value;
                RaisePropertyChanged();
            }
        }

        public LabelPosition LabelPosition {
            get { return _labelPosition; }
            set {
                _labelPosition = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<String> LabelPositions { get; set; }

        public ProjectType ProjectType { get; }

        public RootObject RootObject {
            get { return _rootObject; }
            set {
                _rootObject = value;
                RaisePropertyChanged();
                if (_rootObject == RootObject.None || _rootObject == RootObject.ScrollView) {
                    this.IsWrapGeneratedCodeInBorderEnabled = false;
                } else {
                    this.IsWrapGeneratedCodeInBorderEnabled = true;
                }
            }
        }

        public Boolean ShowTableSectionTitle {
            get { return _showTableSectionTitle; }
            set {
                _showTableSectionTitle = value;
                RaisePropertyChanged();
            }
        }

        public Boolean UiIsXamarinForms => this.ProjectType == ProjectType.Xamarin;

        public Boolean WrapGeneratedCodeInBorder {
            get { return _wrapGeneratedCodeInBorder; }
            set {
                _wrapGeneratedCodeInBorder = value;
                RaisePropertyChanged();
            }
        }

        public GenerateFormModel(ProjectType projectType) {
            if (!Enum.IsDefined(typeof(ProjectType), projectType)) {
                throw new InvalidEnumArgumentException(nameof(projectType), (int)projectType, typeof(ProjectType));
            }
            this.ProjectType = projectType;
            this.LabelPositions = Enum.GetNames(typeof(LabelPosition));
            if (projectType == ProjectType.Xamarin) {
                this.LabelPosition = LabelPosition.Top;
                this.ColumnRootObject = RootObject.StackLayout;
            } else {
                this.RootObject = RootObject.Grid;
                this.ColumnRootObject = RootObject.Grid;
                this.LabelPosition = LabelPosition.Left;
                this.HideRootLayoutOptions = true;
            }
        }

        public void PreGenerateConfiguration() {
            if (this.ProjectType == ProjectType.Wpf) {
                if (this.ColumnLayouts.Count == 1 && String.IsNullOrWhiteSpace(this.FormHeader)) {
                    this.RootObject = RootObject.None;
                } else {
                    this.RootObject = RootObject.Grid;
                }
            }
        }

        void ConfigureLabelPosition() {
            if (this.ColumnRootObject == RootObject.Grid || this.ColumnRootObject == RootObject.TableView) {
                this.LabelPosition = LabelPosition.Left;
                this.IsLabelPositionEnabled = false;
            } else {
                this.IsLabelPositionEnabled = true;
            }
        }

    }
}
