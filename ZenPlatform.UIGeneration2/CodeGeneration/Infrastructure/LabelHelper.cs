using System;
using System.Security;
using System.Text;
using ZenPlatform.UIGeneration2.Infrastructure;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure {
    public class LabelHelper {

        readonly ProjectType _projectType;

        public LabelHelper(ProjectType projectType) {
            _projectType = projectType;
        }

        public String MakeTag(String text, String lableImageName, String labelWidthText, Int32? parentGridColumn, Int32? parentGridRow) {
            if (String.IsNullOrWhiteSpace(text)) {
                return String.Empty;
            }
            var escapedText = SecurityElement.Escape(text);
            var sb = new StringBuilder();

            if (String.IsNullOrWhiteSpace(lableImageName)) {
                if (_projectType == ProjectType.Xamarin) {
                    sb.Append($"<Label Text=\"{escapedText}\" ");
                } else if (_projectType == ProjectType.Uwp || _projectType == ProjectType.Silverlight) {
                    sb.Append($"<TextBlock Text=\"{escapedText}\" ");
                } else {
                    sb.Append($"<Label Content=\"{escapedText}\" ");
                }
            } else {
                sb.Append($"<Image Source=\"{lableImageName}\" ");
            }

            if (parentGridColumn != null && parentGridColumn != 0) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null && parentGridRow != 0) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            if (String.IsNullOrWhiteSpace(lableImageName)) {
                sb.Append(labelWidthText);
            }

            sb.Append("/>");
            return sb.ToString().CompactString();
        }

    }
}
