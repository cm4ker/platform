using System;
using System.Text;
using ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure;
using ZenPlatform.UIGeneration2.Infrastructure;
using ZenPlatform.UIGeneration2.Wpf;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Wpf.Controls {
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
                sb.AppendFormat("SelectedDate=\"{0}\" ", Helpers.ConstructBinding(_model.BindingPath, _model.BindingMode, String.Empty, _model.BindingConverter, IncludeValidationAttributes.Yes));
            }

            if (!String.IsNullOrWhiteSpace(_model.StringFormat)) {
                sb.Append("SelectedDateFormat=\"Short\" ");
            }

            if (!String.IsNullOrWhiteSpace(_model.EditorProperties.DisplayDateStartPathText)) {
                sb.AppendFormat("DisplayDateStart=\"{0}\" ", Helpers.ConstructBinding(_model.EditorProperties.DisplayDateStartPathText, BindingMode.OneWay, String.Empty, _model.BindingConverter));
            } else if (_model.EditorProperties.SetDisplayDatStartToDateTimeNow) {
                sb.Append($"DisplayDateStart=\"{{x:Static sys:DateTime.Now}}\" ");
            } else if (_model.EditorProperties.DisplayDateStart.HasValue) {
                sb.Append($"DisplayDateStart=\"{_model.EditorProperties.DisplayDateStart.Value.ToShortDateString()}\" ");
            }

            if (!String.IsNullOrWhiteSpace(_model.EditorProperties.DisplayDateEndPathText)) {
                sb.AppendFormat("DisplayDateEnd=\"{0}\" ", Helpers.ConstructBinding(_model.EditorProperties.DisplayDateEndPathText, BindingMode.OneWay, String.Empty, _model.BindingConverter));
            } else if (_model.EditorProperties.SetDisplayDateEndToDateTimeNow) {
                sb.Append($"DisplayDateEnd=\"{{x:Static sys:DateTime.Now}}\" ");
            } else if (_model.EditorProperties.DisplayDateEnd.HasValue) {
                sb.Append($"DisplayDateEnd=\"{_model.EditorProperties.DisplayDateEnd.Value.ToShortDateString()}\" ");
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
