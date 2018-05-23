using System;
using System.Text;
using ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure;
using ZenPlatform.UIGeneration2.Infrastructure;
using ZenPlatform.UIGeneration2.XamarinForms;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls {
    public class BindablePickerFactory : IControlFactory {

        readonly ControlTemplateModel<BindablePickerEditorProperties> _model;

        public BindablePickerFactory(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (propertyInformationViewModel == null) {
                throw new ArgumentNullException(nameof(propertyInformationViewModel));
            }
            _model = new ControlTemplateModel<BindablePickerEditorProperties>(generateFormModel, propertyInformationViewModel);
        }


        public string MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<controls:BindablePicker ");

            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            sb.Append(_model.EditorProperties.ItemsSourceText);
            sb.Append(_model.EditorProperties.DisplayMemberPathText);
            sb.Append(_model.EditorProperties.SelectedItemPathText);
            sb.Append(_model.EditorProperties.SelectedValuePathText);
            sb.Append(_model.EditorProperties.TitleText);

            if (!String.IsNullOrWhiteSpace(_model.BindingPath)) {
                var editorBindingTarget = _model.EditorProperties.EditorBindingTargetProperty;
                if (String.IsNullOrWhiteSpace(editorBindingTarget)) {
                    editorBindingTarget = "SelectedItem";
                }
                sb.AppendFormat("{0}=\"{1}\" ", editorBindingTarget, Helpers.ConstructBinding(_model.BindingPath, _model.BindingMode, _model.StringFormatText, _model.BindingConverter));
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