using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls;

namespace ZenPlatform.UIGeneration2.XamarinForms {
    [Serializable]
    public class DatePickerEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        DateTime? _maximumDate;
        String _maximumDatePathText;
        DateTime? _minimumDate;
        String _minimumDatePathText;
        Boolean _setMaximumDateToDateTimeNow;
        Boolean _setMinimumDateToDateTimeNow;

        public DateTime? MaximumDate {
            get { return _maximumDate; }
            set {
                _maximumDate = value;
                RaisePropertyChanged();

                _setMaximumDateToDateTimeNow = false;
                _maximumDatePathText = String.Empty;
                RaisePropertyChanged("SetMaximumDateToDateTimeNow");
                RaisePropertyChanged("MaximumDatePathText");
            }
        }

        public String MaximumDatePathText {
            get { return _maximumDatePathText; }
            set {
                _maximumDatePathText = value;
                RaisePropertyChanged();

                _setMaximumDateToDateTimeNow = false;
                _maximumDate = null;
                RaisePropertyChanged("SetMaximumDateToDateTimeNow");
                RaisePropertyChanged("MaximumDate");
            }
        }

        public DateTime? MinimumDate {
            get { return _minimumDate; }
            set {
                _minimumDate = value;
                RaisePropertyChanged();

                _setMinimumDateToDateTimeNow = false;
                _minimumDatePathText = String.Empty;
                RaisePropertyChanged("SetMinimumDateToDateTimeNow");
                RaisePropertyChanged("MinimumDatePathText");
            }
        }

        public String MinimumDatePathText {
            get { return _minimumDatePathText; }
            set {
                _minimumDatePathText = value;
                RaisePropertyChanged();

                _setMinimumDateToDateTimeNow = false;
                _minimumDate = null;
                RaisePropertyChanged("SetMinimumDateToDateTimeNow");
                RaisePropertyChanged("MinimumDate");
            }
        }

        public Boolean SetMaximumDateToDateTimeNow {
            get { return _setMaximumDateToDateTimeNow; }
            set {
                _setMaximumDateToDateTimeNow = value;
                RaisePropertyChanged();

                _maximumDate = null;
                _maximumDatePathText = String.Empty;
                RaisePropertyChanged("MaximumDate");
                RaisePropertyChanged("MaximumDatePathText");
            }
        }

        public Boolean SetMinimumDateToDateTimeNow {
            get { return _setMinimumDateToDateTimeNow; }
            set {
                _setMinimumDateToDateTimeNow = value;
                RaisePropertyChanged();

                _minimumDate = null;
                _minimumDatePathText = String.Empty;
                RaisePropertyChanged("MinimumDate");
                RaisePropertyChanged("MinimumDatePathText");
            }
        }

        public String TemplateResourceKey { get; }

        public DatePickerEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsDatePickerEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new DatePickerFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
