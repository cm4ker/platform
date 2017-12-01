using System;
using System.Text;
using ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure;
using ZenPlatform.UIGeneration2.Infrastructure;
using ZenPlatform.UIGeneration2.XamarinForms;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls {
    public class DatePickerFactory : IControlFactory {

        readonly ControlTemplateModel<DatePickerEditorProperties> _model;

        public DatePickerFactory(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (propertyInformationViewModel == null) {
                throw new ArgumentNullException(nameof(propertyInformationViewModel));
            }
            _model = new ControlTemplateModel<DatePickerEditorProperties>(generateFormModel, propertyInformationViewModel);
        }

        public String MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<DatePicker ");
            if (!String.IsNullOrWhiteSpace(_model.BindingPath)) {
                sb.AppendFormat("Date=\"{0}\" ", Helpers.ConstructBinding(_model.BindingPath, _model.BindingMode, _model.StringFormatText, _model.BindingConverter));
            }

            if (!String.IsNullOrWhiteSpace(_model.StringFormat)) {
                sb.AppendFormat("Format=\"{0}\" ", _model.StringFormat);
            }

            if (!String.IsNullOrWhiteSpace(_model.EditorProperties.MaximumDatePathText)) {
                sb.AppendFormat("MaximumDate=\"{0}\" ", Helpers.ConstructBinding(_model.EditorProperties.MaximumDatePathText, BindingMode.OneWay, String.Empty, _model.BindingConverter));
            } else if (_model.EditorProperties.SetMaximumDateToDateTimeNow) {
                sb.Append($"MaximumDate=\"{{x:Static sys:DateTime.Now}}\" ");
            } else if (_model.EditorProperties.MaximumDate.HasValue) {
                sb.Append($"MinimumDate=\"{_model.EditorProperties.MaximumDate.Value.ToShortDateString()}\" ");
            }

            if (!String.IsNullOrWhiteSpace(_model.EditorProperties.MinimumDatePathText)) {
                sb.AppendFormat("MinimumDate=\"{0}\" ", Helpers.ConstructBinding(_model.EditorProperties.MinimumDatePathText, BindingMode.OneWay, String.Empty, _model.BindingConverter));
            } else if (_model.EditorProperties.SetMinimumDateToDateTimeNow) {
                sb.Append($"MinimumDate=\"{{x:Static sys:DateTime.Now}}\" ");
            } else if (_model.EditorProperties.MinimumDate.HasValue) {
                sb.Append($"MinimumDate=\"{_model.EditorProperties.MinimumDate.Value.ToShortDateString()}\" ");
            }

            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            sb.Append(_model.WidthText);
            sb.Append(_model.HeightText);
            sb.Append(_model.ControlNameText);
            sb.Append("/>");

            return sb.ToString().CompactString();
        }

    }
}
