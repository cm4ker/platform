using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ZenPlatform.UIGeneration2.Infrastructure;
using ZenPlatform.UIGeneration2.System.Windows;
using ZenPlatform.UIGeneration2.Uwp;
using ZenPlatform.UIGeneration2.XamarinForms;
using ButtonEditorProperties = ZenPlatform.UIGeneration2.Silverlight.ButtonEditorProperties;
using CheckBoxEditorProperties = ZenPlatform.UIGeneration2.Silverlight.CheckBoxEditorProperties;
using ComboBoxEditorProperties = ZenPlatform.UIGeneration2.Silverlight.ComboBoxEditorProperties;
using DatePickerEditorProperties = ZenPlatform.UIGeneration2.Silverlight.DatePickerEditorProperties;
using ImageEditorProperties = ZenPlatform.UIGeneration2.Silverlight.ImageEditorProperties;
using LabelEditorProperties = ZenPlatform.UIGeneration2.Wpf.LabelEditorProperties;
using SliderEditorProperties = ZenPlatform.UIGeneration2.Silverlight.SliderEditorProperties;
using TextBlockEditorProperties = ZenPlatform.UIGeneration2.Silverlight.TextBlockEditorProperties;
using TextBoxEditorProperties = ZenPlatform.UIGeneration2.Silverlight.TextBoxEditorProperties;

namespace ZenPlatform.UIGeneration2 {
    [Serializable]
    public class PropertyInformationViewModel : ObservableObject, IComparable<PropertyInformationViewModel>, IEquatable<PropertyInformationViewModel> {

        String _bindingConverter = String.Empty;
        BindingMode _bindingMode = BindingMode.Default;
        String _cellWidthText;
        ControlDefinition _controlDefinition;
        Object _controlSpecificProperties;
        Int32? _height;
        Boolean _includeNextControlInRow;
        String _labelImageName;
        String _labelText;
        Int32? _labelWidth;
        Int32? _maximumLength;
        readonly ProjectType _projectType;
        readonly String _projectTypeName;
        Boolean _showLabel = true;
        Boolean _showStringFormatProperty;
        String _stringFormat = String.Empty;
        String _tableSectionTitle = "CHANGE";
        Int32? _width;

        public String BindingConverter {
            get { return _bindingConverter; }
            set {
                _bindingConverter = value;
                RaisePropertyChanged();
            }
        }

        public BindingMode BindingMode {
            get { return _bindingMode; }
            set {
                _bindingMode = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<String> BindingModes { get; private set; }

        public String BindingPath { get; }

        public Boolean CanWrite { get; }

        public GridLength? CellWidthGridLength { get; private set; }

        public String CellWidthText {
            get { return _cellWidthText; }
            set {
                if (String.IsNullOrWhiteSpace(value)) {
                    _cellWidthText = value;
                    this.CellWidthGridLength = null;
                    RaisePropertyChanged();
                    return;
                }
                var converter = new GridLengthConverter();

                if (converter.IsValid(value)) {
                    var convertFromString = converter.ConvertFromString(value);
                    if (convertFromString != null) {
                        this.CellWidthGridLength = (GridLength)convertFromString;
                    } else {
                        this.CellWidthGridLength = new GridLength();
                    }
                    _cellWidthText = value;
                    RaisePropertyChanged();
                } else {
                    throw new ArgumentException("Entered value is not a grid length. Example:  3, *, .5*");
                }
            }
        }

        public ClassEntity ClassEntity { get; set; }

        public ControlDefinition ControlDefinition {
            get { return _controlDefinition; }
            set {
                _controlDefinition = value;
                RaisePropertyChanged();
                SetControlSpecificPropertiesObject();
            }
        }

        public IEnumerable<ControlDefinition> ControlDefinitions { get; }

        public Object ControlSpecificProperties {
            get { return _controlSpecificProperties; }
            private set {
                _controlSpecificProperties = value;
                RaisePropertyChanged();
            }
        }

        public String DefaultLabelText { get; }

        public String FullName => $"{this.Name}{(this.CanWrite ? String.Empty : " (r)")} - {this.TypeName}";

        public List<String> GenericArguments { get; set; }

        public IList<String> GenericCollectionClassPropertyNames { get; set; } = new List<String>();

        public Int32? Height {
            get { return _height; }
            set {
                _height = value;
                RaisePropertyChanged();
            }
        }

        public Boolean IncludeNextControlInRow {
            get { return _includeNextControlInRow; }
            set {
                _includeNextControlInRow = value;
                RaisePropertyChanged();
                if (_includeNextControlInRow && String.IsNullOrWhiteSpace(_cellWidthText)) {
                    this.CellWidthText = "Auto";
                } else if (!_includeNextControlInRow) {
                    this.CellWidthText = String.Empty;
                }
            }
        }

        public Boolean IsBindingModeSelectionEnabled => !this.IsNonBindingControl;

        public Boolean IsControlSelectionEnabled => !this.IsNonBindingControl && this.ControlDefinition.ShortName != "Button";

        public Boolean IsDrillableProperty => this.ClassEntity != null;

        public Boolean IsEnumerable { get; }

        public Boolean IsNonBindingControl { get; }

        public Boolean IsReadOnly => !this.CanWrite;

        public IEnumerable<String> Keyboards { get; }

        public String LabelImageName {
            get { return _labelImageName; }
            set {
                _labelImageName = value;
                RaisePropertyChanged();
            }
        }

        public String LabelText {
            get { return _labelText; }
            set {
                _labelText = value;
                RaisePropertyChanged();
            }
        }

        public Int32? LabelWidth {
            get { return _labelWidth; }
            set {
                _labelWidth = value;
                RaisePropertyChanged();
            }
        }

        public Int32? MaximumLength {
            get { return _maximumLength; }
            set {
                _maximumLength = value;
                RaisePropertyChanged();
            }
        }

        public String Name { get; }

        public String NameAndWritable => this.CanWrite ? this.Name : $"{this.Name}  (r)";

        public String NamespaceName { get; }

        public String NamespaceTypeName => String.Concat(this.NamespaceName, ":", this.TypeName, this.IsDrillableProperty ? "  -  double click BOLD type name to drill into this type." : "");

        public String ParentClassName { get; }

        public String ParentPropertyName { get; }

        public List<PropertyParameter> PropertyParameters { get; }

        public String RadioButtonGroupName => Guid.NewGuid().ToString();

        public Boolean ShowLabel {
            get { return _showLabel; }
            set {
                _showLabel = value;
                RaisePropertyChanged();
            }
        }

        public Boolean ShowStringFormatProperty {
            get { return _showStringFormatProperty; }
            set {
                _showStringFormatProperty = value;
                RaisePropertyChanged();
            }
        }

        public String StringFormat {
            get { return _stringFormat; }
            set {
                _stringFormat = value;
                RaisePropertyChanged();
            }
        }

        public String TableSectionTitle {
            get { return _tableSectionTitle; }
            set {
                _tableSectionTitle = value;
                RaisePropertyChanged();
            }
        }

        public String TypeName { get; }

        public Int32? Width {
            get { return _width; }
            set {
                _width = value;
                RaisePropertyChanged();
            }
        }

        public PropertyInformationViewModel(Boolean canWrite, String name, String typeName, String namespaceName, ProjectType projectType, String parentClassName, Boolean isEnumerable, Boolean isNonBindingControl, String parentPropertyName = "") {
            if (String.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }
            if (!isNonBindingControl && String.IsNullOrWhiteSpace(typeName)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(typeName));
            }
            if (!isNonBindingControl && String.IsNullOrWhiteSpace(namespaceName)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(namespaceName));
            }
            if (!Enum.IsDefined(typeof(ProjectType), projectType)) {
                throw new InvalidEnumArgumentException(nameof(projectType), (Int32)projectType, typeof(ProjectType));
            }
            if (!isNonBindingControl && String.IsNullOrWhiteSpace(parentClassName)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(parentClassName));
            }

            this.CanWrite = canWrite;
            this.Name = name;
            this.TypeName = typeName;
            this.NamespaceName = namespaceName;
            this.ParentClassName = parentClassName;
            this.IsEnumerable = isEnumerable;
            this.IsNonBindingControl = isNonBindingControl;
            this.ParentPropertyName = parentPropertyName;
            _projectType = projectType;
            _projectTypeName = projectType.ToString();

            this.GenericArguments = new List<String>();
            this.PropertyParameters = new List<PropertyParameter>();

            this.DefaultLabelText = this.Name.SplitWords();
            this.LabelText = this.DefaultLabelText;
            if (!isNonBindingControl) {
                this.BindingPath = !String.IsNullOrWhiteSpace(parentPropertyName) ? $"{parentPropertyName}.{this.Name}" : this.Name;
            }

            this.ControlDefinitions = GetControlControlDefinitions();
            SetBindingModes();
            this.Keyboards = new List<String> {"Default", "Chat", "Email", "Numeric", "Telephone", "Text", "Url"};
            SetDefaultControlDefinition();
        }

        public Int32 CompareTo(PropertyInformationViewModel other) {
            if (this.Name == other.Name) {
                return 0;
            }
            return String.Compare(this.Name, other.Name, StringComparison.Ordinal);
        }

        public Boolean Equals(PropertyInformationViewModel other) {
            if (other == null) {
                return false;
            }
            return this.Name.Equals(other.Name);
        }

        public IConstructControlFactory GetTemplateFactory() {
            return (IConstructControlFactory)this.ControlSpecificProperties;
        }

        public void ResetUserEnteredValues() {
            _bindingMode = BindingMode.Default;
            _showLabel = true;
            _stringFormat = String.Empty;
            this.ControlSpecificProperties = null;
            this.LabelText = this.DefaultLabelText;
            SetDefaultControlDefinition();
        }

        IEnumerable<ControlDefinition> GetControlControlDefinitions() {
            var list = new List<ControlDefinition>();
            foreach (var name in Enum.GetNames(typeof(ControlType))) {
                if (name.StartsWith(_projectTypeName)) {
                    list.Add(new ControlDefinition((ControlType)Enum.Parse(typeof(ControlType), name)));
                }
            }
            return list;
        }

        void SetBindingModes() {
            if (!this.IsNonBindingControl) {
                var list = new List<String>();
                foreach (var item in Enum.GetNames(typeof(BindingMode))) {
                    if (_projectType == ProjectType.Uwp || _projectType == ProjectType.Silverlight) {
                        if (item == "Default" || item == "OneWayToSource") {
                            continue;
                        }
                    } else if (_projectType == ProjectType.Xamarin) {
                        if (item == "OneTime") {
                            continue;
                        }
                    }
                    list.Add(item);
                }
                this.BindingModes = list;
            }
        }

        void SetControlSpecificPropertiesObject() {
            if (this.TypeName.Contains("Date")) {
                this.StringFormat = _projectType == ProjectType.Xamarin ? "D" : "{0:d}";
            } else {
                this.StringFormat = String.Empty;
            }
            switch (_projectType) {
                case ProjectType.Uwp:
                    SetUwpControlSpecificPropertiesObject();
                    break;
                case ProjectType.Wpf:
                    SetWpfControlSpecificPropertiesObject();
                    break;
                case ProjectType.Xamarin:
                    SetXamarinControlSpecificPropertiesObject();
                    break;
                case ProjectType.Silverlight:
                    SetSilverlightControlSpecificPropertiesObject();
                    break;
            }
        }

        void SetDefaultControlDefinition() {
            switch (_projectType) {
                case ProjectType.Uwp:
                    SetUwpDefaultControlDefinition();
                    break;
                case ProjectType.Wpf:
                    SetWpfDefaultControlDefinition();
                    break;
                case ProjectType.Xamarin:
                    SetXamarinDefaultControlDefinition();
                    break;
                case ProjectType.Silverlight:
                    SetSilverlightDefaultControlDefinition();
                    break;
            }
        }

        void SetSilverlightControlSpecificPropertiesObject() {
            this.ShowStringFormatProperty = false;

            switch (this.ControlDefinition.ControlType) {
                case ControlType.SilverlightButton:
                    this.ControlSpecificProperties = new ButtonEditorProperties();
                    break;
                case ControlType.SilverlightCheckBox:
                    this.ControlSpecificProperties = new CheckBoxEditorProperties();
                    break;
                case ControlType.SilverlightComboBox:
                    this.ControlSpecificProperties = new ComboBoxEditorProperties();
                    break;
                case ControlType.SilverlightDatePicker:
                    this.ControlSpecificProperties = new DatePickerEditorProperties();
                    break;
                case ControlType.SilverlightImage:
                    this.ControlSpecificProperties = new ImageEditorProperties();
                    break;
                case ControlType.SilverlightSlider:
                    this.ControlSpecificProperties = new SliderEditorProperties();
                    break;
                case ControlType.SilverlightTextBlock:
                    this.ShowStringFormatProperty = true;
                    this.ControlSpecificProperties = new TextBlockEditorProperties();
                    break;
                case ControlType.SilverlightTextBox:
                    this.ShowStringFormatProperty = true;
                    this.ControlSpecificProperties = new TextBoxEditorProperties();
                    break;
            }
        }

        void SetSilverlightDefaultControlDefinition() {
            if (this.IsNonBindingControl) {
                this.LabelText = String.Empty;
                this.BindingMode = BindingMode.Default;
                return;
            }

            if (!CanWrite && this.TypeName != "ICommand") {
                this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.SilverlightTextBlock);
                return;
            }

            switch (this.TypeName) {
                case "DateTime":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.SilverlightDatePicker);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case "Boolean":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.SilverlightCheckBox);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case "ICommand":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.SilverlightButton);
                    var buttonEditorProperties = (ButtonEditorProperties)this.ControlSpecificProperties;
                    buttonEditorProperties.Command = this.BindingPath;
                    buttonEditorProperties.Content = this.BindingPath.Replace("Command", String.Empty);
                    break;
                default:
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.SilverlightTextBox);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
            }
        }

        void SetUwpControlSpecificPropertiesObject() {
            this.ShowStringFormatProperty = false;
            switch (this.ControlDefinition.ControlType) {
                case ControlType.UwpButton:
                    this.ControlSpecificProperties = new Uwp.ButtonEditorProperties();
                    this.BindingMode = BindingMode.OneWay;
                    break;
                case ControlType.UwpCheckBox:
                    this.ControlSpecificProperties = new Uwp.CheckBoxEditorProperties();
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case ControlType.UwpComboBox:
                    this.ControlSpecificProperties = new Uwp.ComboBoxEditorProperties();
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case ControlType.UwpDatePicker:
                    this.ControlSpecificProperties = new Uwp.DatePickerEditorProperties();
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case ControlType.UwpImage:
                    this.ControlSpecificProperties = new Uwp.ImageEditorProperties();
                    this.BindingMode = BindingMode.OneWay;
                    break;
                case ControlType.UwpSlider:
                    this.ControlSpecificProperties = new Uwp.SliderEditorProperties();
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case ControlType.UwpTextBlock:
                    this.ShowStringFormatProperty = true;
                    this.ControlSpecificProperties = new Uwp.TextBlockEditorProperties();
                    this.BindingMode = BindingMode.OneWay;
                    break;
                case ControlType.UwpTextBox:
                    this.ShowStringFormatProperty = true;
                    this.ControlSpecificProperties = new Uwp.TextBoxEditorProperties();
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case ControlType.UwpToggleButton:
                    this.ControlSpecificProperties = new ToggleButtonEditorProperties();
                    this.BindingMode = BindingMode.TwoWay;
                    break;
            }
        }

        void SetUwpDefaultControlDefinition() {
            this.BindingMode = BindingMode.OneWay;

            if (this.IsNonBindingControl) {
                this.LabelText = "";
                return;
            }

            if (!CanWrite && this.TypeName != "ICommand") {
                this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.UwpTextBlock);
                this.BindingMode = BindingMode.OneWay;
                return;
            }

            switch (this.TypeName) {
                case "DateTimeOffset":
                case "DateTime":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.UwpDatePicker);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case "Boolean":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.UwpCheckBox);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case "ICommand":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.UwpButton);
                    var buttonEditorProperties = (Uwp.ButtonEditorProperties)this.ControlSpecificProperties;
                    buttonEditorProperties.Command = this.BindingPath;
                    buttonEditorProperties.Content = this.BindingPath.Replace("Command", String.Empty);
                    this.BindingMode = BindingMode.OneWay;
                    break;
                default:
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.UwpTextBox);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
            }
        }

        void SetWpfControlSpecificPropertiesObject() {
            this.ShowStringFormatProperty = false;

            switch (this.ControlDefinition.ControlType) {
                case ControlType.WpfButton:
                    this.ControlSpecificProperties = new Wpf.ButtonEditorProperties();
                    break;
                case ControlType.WpfCheckBox:
                    this.ControlSpecificProperties = new Wpf.CheckBoxEditorProperties();
                    break;
                case ControlType.WpfComboBox:
                    this.ControlSpecificProperties = new Wpf.ComboBoxEditorProperties();
                    break;
                case ControlType.WpfDatePicker:
                    this.ControlSpecificProperties = new Wpf.DatePickerEditorProperties();
                    break;
                case ControlType.WpfImage:
                    this.ControlSpecificProperties = new Wpf.ImageEditorProperties();
                    break;
                case ControlType.WpfLabel:
                    this.ShowStringFormatProperty = true;
                    this.ControlSpecificProperties = new LabelEditorProperties();
                    break;
                case ControlType.WpfSlider:
                    this.ControlSpecificProperties = new Wpf.SliderEditorProperties();
                    break;
                case ControlType.WpfTextBlock:
                    this.ShowStringFormatProperty = true;
                    this.ControlSpecificProperties = new Wpf.TextBlockEditorProperties();
                    break;
                case ControlType.WpfTextBox:
                    this.ShowStringFormatProperty = true;
                    this.ControlSpecificProperties = new Wpf.TextBoxEditorProperties();
                    break;
            }
        }

        void SetWpfDefaultControlDefinition() {
            if (this.IsNonBindingControl) {
                this.LabelText = String.Empty;
                this.BindingMode = BindingMode.Default;
                return;
            }

            if (!CanWrite && this.TypeName != "ICommand") {
                this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.WpfTextBlock);
                return;
            }

            switch (this.TypeName) {
                case "DateTime":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.WpfDatePicker);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case "Boolean":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.WpfCheckBox);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case "ICommand":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.WpfButton);
                    var buttonEditorProperties = (Wpf.ButtonEditorProperties)this.ControlSpecificProperties;
                    buttonEditorProperties.Command = this.BindingPath;
                    buttonEditorProperties.Content = this.BindingPath.Replace("Command", String.Empty);
                    break;
                default:
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.WpfTextBox);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
            }
        }

        void SetXamarinControlSpecificPropertiesObject() {
            this.ShowStringFormatProperty = false;
            var labelText = this.LabelText;
            if (this.IsNonBindingControl) {
                labelText = String.Empty;
            }
            switch (this.ControlDefinition.ControlType) {
                case ControlType.XamarinBindablePicker:
                    this.ControlSpecificProperties = new BindablePickerEditorProperties();
                    break;
                case ControlType.XamarinButton:
                    this.ControlSpecificProperties = new XamarinForms.ButtonEditorProperties();
                    break;
                case ControlType.XamarinDatePicker:
                    this.ShowStringFormatProperty = true;
                    this.ControlSpecificProperties = new XamarinForms.DatePickerEditorProperties();
                    break;
                case ControlType.XamarinEditor:
                    this.ControlSpecificProperties = new EditorEditorProperties();
                    break;
                case ControlType.XamarinEntry:
                    this.ShowStringFormatProperty = true;
                    var isPassword = this.BindingPath != null && this.BindingPath.IndexOf("password", StringComparison.OrdinalIgnoreCase) != -1;
                    if (this.IsNonBindingControl) {
                        isPassword = false;
                    }
                    this.ControlSpecificProperties = new EntryEditorProperties {Placeholder = labelText, IsPassword = isPassword};
                    break;
                case ControlType.XamarinImage:
                    this.ControlSpecificProperties = new XamarinForms.ImageEditorProperties();
                    break;
                case ControlType.XamarinLabel:
                    this.ShowStringFormatProperty = true;
                    this.ControlSpecificProperties = new XamarinForms.LabelEditorProperties();
                    break;
                case ControlType.XamarinPicker:
                    this.ControlSpecificProperties = new PickerEditorProperties();
                    break;
                case ControlType.XamarinSlider:
                    this.ControlSpecificProperties = new XamarinForms.SliderEditorProperties();
                    break;
                case ControlType.XamarinStepper:
                    this.ControlSpecificProperties = new StepperEditorProperties();
                    break;
                case ControlType.XamarinSwitch:
                    this.ControlSpecificProperties = new SwitchEditorProperties();
                    break;
                case ControlType.XamarinTimePicker:
                    this.ShowStringFormatProperty = true;
                    this.ControlSpecificProperties = new TimePickerEditorProperties();
                    break;
            }
        }

        void SetXamarinDefaultControlDefinition() {
            if (this.IsNonBindingControl) {
                this.LabelText = "";
                this.BindingMode = BindingMode.Default;
                return;
            }

            if (!CanWrite && this.TypeName != "ICommand") {
                this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.XamarinLabel);
                return;
            }

            switch (this.TypeName) {
                case "DateTime":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.XamarinDatePicker);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case "Boolean":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.XamarinSwitch);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case "TimeSpan":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.XamarinTimePicker);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
                case "ICommand":
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.XamarinButton);
                    var buttonEditorProperties = (XamarinForms.ButtonEditorProperties)this.ControlSpecificProperties;
                    buttonEditorProperties.Command = this.BindingPath;
                    buttonEditorProperties.Text = this.BindingPath.Replace("Command", String.Empty);
                    break;
                default:
                    this.ControlDefinition = this.ControlDefinitions.First(x => x.ControlType == ControlType.XamarinEntry);
                    this.BindingMode = BindingMode.TwoWay;
                    break;
            }
        }

    }
}
