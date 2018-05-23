using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Silverlight.Controls;

namespace ZenPlatform.UIGeneration2.Silverlight {
    [Serializable]
    public class DatePickerEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        DateTime? _displayDateEnd;
        String _displayDateEndPathText;
        DateTime? _displayDateStart;
        String _displayDateStartPathText;
        Boolean _setDisplayDateEndToDateTimeNow;
        Boolean _setDisplayDatStartToDateTimeNow;

        public DateTime? DisplayDateEnd {
            get { return _displayDateEnd; }
            set {
                _displayDateEnd = value;
                RaisePropertyChanged();

                _setDisplayDateEndToDateTimeNow = false;
                _displayDateEndPathText = String.Empty;
                RaisePropertyChanged("SetDisplayDateEndToDateTimeNow");
                RaisePropertyChanged("DisplayDateEndPathText");
            }
        }

        public String DisplayDateEndPathText {
            get { return _displayDateEndPathText; }
            set {
                _displayDateEndPathText = value;
                RaisePropertyChanged();

                _setDisplayDateEndToDateTimeNow = false;
                _displayDateEnd = null;
                RaisePropertyChanged("SetDisplayDateEndToDateTimeNow");
                RaisePropertyChanged("DisplayDateEnd");
            }
        }

        public DateTime? DisplayDateStart {
            get { return _displayDateStart; }
            set {
                _displayDateStart = value;
                RaisePropertyChanged();

                _setDisplayDatStartToDateTimeNow = false;
                _displayDateStartPathText = String.Empty;

                RaisePropertyChanged("SetDisplayDatStartToDateTimeNow");
                RaisePropertyChanged("DisplayDateStartPathText");
            }
        }

        public String DisplayDateStartPathText {
            get { return _displayDateStartPathText; }
            set {
                _displayDateStartPathText = value;
                RaisePropertyChanged();

                _setDisplayDatStartToDateTimeNow = false;
                _displayDateStart = null;
                RaisePropertyChanged("SetDisplayDatStartToDateTimeNow");
                RaisePropertyChanged("DisplayDateStart");
            }
        }

        public Boolean SetDisplayDateEndToDateTimeNow {
            get { return _setDisplayDateEndToDateTimeNow; }
            set {
                _setDisplayDateEndToDateTimeNow = value;
                RaisePropertyChanged();

                _displayDateEnd = null;
                _displayDateEndPathText = String.Empty;
                RaisePropertyChanged("DisplayDateEnd");
                RaisePropertyChanged("DisplayDateEndPathText");
            }
        }

        public Boolean SetDisplayDatStartToDateTimeNow {
            get { return _setDisplayDatStartToDateTimeNow; }
            set {
                _setDisplayDatStartToDateTimeNow = value;
                RaisePropertyChanged();

                _displayDateStart = null;
                _displayDateStartPathText = String.Empty;
                RaisePropertyChanged("DisplayDateStart");
                RaisePropertyChanged("DisplayDateStartPathText");
            }
        }

        public String TemplateResourceKey { get; }

        public DatePickerEditorProperties() {
            this.TemplateResourceKey = "silverlightDatePickerEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new DatePickerFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
