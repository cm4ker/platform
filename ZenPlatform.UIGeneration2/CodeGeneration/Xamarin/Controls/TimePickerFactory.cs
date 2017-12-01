using System;
using System.Text;
using ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure;
using ZenPlatform.UIGeneration2.Infrastructure;
using ZenPlatform.UIGeneration2.XamarinForms;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls {
    public class TimePickerFactory : IControlFactory {

        readonly ControlTemplateModel<TimePickerEditorProperties> _model;

        public TimePickerFactory(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (propertyInformationViewModel == null) {
                throw new ArgumentNullException(nameof(propertyInformationViewModel));
            }
            _model = new ControlTemplateModel<TimePickerEditorProperties>(generateFormModel, propertyInformationViewModel);
        }

        public string MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<TimePicker ");
            if (!String.IsNullOrWhiteSpace(_model.BindingPath)) {
                sb.AppendFormat("Time=\"{0}\" ", Helpers.ConstructBinding(_model.BindingPath, _model.BindingMode, String.Empty, _model.BindingConverter));
            }

            if (!String.IsNullOrWhiteSpace(_model.StringFormat)) {
                sb.AppendFormat("Format=\"{0}\" ", _model.StringFormat);
            }

            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            sb.Append(_model.WidthText);
            sb.Append(_model.HeightText);
            sb.Append(_model.HorizontalAlignmentText);
            sb.Append(_model.VerticalAlignmentText);
            sb.Append(_model.ControlNameText);
            sb.Append("/>");
            return sb.ToString().CompactString();
        }

    }
}
