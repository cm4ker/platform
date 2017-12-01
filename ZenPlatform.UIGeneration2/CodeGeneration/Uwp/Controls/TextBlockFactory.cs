using System;
using System.Text;
using ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure;
using ZenPlatform.UIGeneration2.Infrastructure;
using ZenPlatform.UIGeneration2.Uwp;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Uwp.Controls {
    public class TextBlockFactory : IControlFactory {

        readonly ControlTemplateModel<TextBlockEditorProperties> _model;

        public TextBlockFactory(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (propertyInformationViewModel == null) {
                throw new ArgumentNullException(nameof(propertyInformationViewModel));
            }
            _model = new ControlTemplateModel<TextBlockEditorProperties>(generateFormModel, propertyInformationViewModel);
        }

        public String MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<TextBlock ");
            if (!String.IsNullOrWhiteSpace(_model.BindingPath)) {
                sb.AppendFormat("Text=\"{0}\" ", Helpers.ConstructBinding(_model.BindingPath, BindingMode.OneWay, _model.StringFormatText, _model.BindingConverter));
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