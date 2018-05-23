using System;
using System.Text;
using ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure;
using ZenPlatform.UIGeneration2.Infrastructure;
using ZenPlatform.UIGeneration2.XamarinForms;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls {
    public class StepperFactory : IControlFactory {

        readonly ControlTemplateModel<StepperEditorProperties> _model;

        public StepperFactory(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (propertyInformationViewModel == null) {
                throw new ArgumentNullException(nameof(propertyInformationViewModel));
            }
            _model = new ControlTemplateModel<StepperEditorProperties>(generateFormModel, propertyInformationViewModel);
        }

        public String MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<Stepper ");
            if (!String.IsNullOrWhiteSpace(_model.BindingPath)) {
                sb.AppendFormat("Value=\"{0}\" ", Helpers.ConstructBinding(_model.BindingPath, _model.BindingMode, _model.StringFormatText, _model.BindingConverter));
            }

            if (!String.IsNullOrWhiteSpace(_model.EditorProperties.MaximumPathText)) {
                sb.AppendFormat("Maximum=\"{0}\" ", Helpers.ConstructBinding(_model.EditorProperties.MaximumPathText, _model.BindingMode, String.Empty, _model.BindingConverter));
            } else {
                sb.Append($"Maximum=\"{_model.EditorProperties.Maximum}\" ");
            }

            if (!String.IsNullOrWhiteSpace(_model.EditorProperties.MinimumPathText)) {
                sb.AppendFormat("Minimum=\"{0}\" ", Helpers.ConstructBinding(_model.EditorProperties.MinimumPathText, _model.BindingMode, String.Empty, _model.BindingConverter));
            } else {
                sb.Append($"Minimum=\"{_model.EditorProperties.Minimum}\" ");
            }

            if (!String.IsNullOrWhiteSpace(_model.EditorProperties.IncrementPathText)) {
                sb.AppendFormat("Increment=\"{0}\" ", Helpers.ConstructBinding(_model.EditorProperties.IncrementPathText, _model.BindingMode, String.Empty, _model.BindingConverter));
            } else {
                sb.Append($"Increment=\"{_model.EditorProperties.Increment}\" ");
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
