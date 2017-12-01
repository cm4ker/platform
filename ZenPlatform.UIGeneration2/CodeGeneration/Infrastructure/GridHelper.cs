using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.UIGeneration2.Infrastructure;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure {
    public class GridHelper {

        public GridHelper() {
        }

        public String EndTag() {
            return "</Grid>";
        }

        public String StartTag(IList<GridLength> gridColumns, IList<GridLength> gridRows, Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<Grid ");
            if (parentGridColumn != null && parentGridColumn.Value > 0) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null && parentGridRow.Value > 0) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }
            sb.AppendLine(">");

            if (gridColumns != null && gridColumns.Count > 0) {
                sb.AppendLine("<Grid.ColumnDefinitions>");
                foreach (var column in gridColumns) {
                    sb.AppendLine($"<ColumnDefinition Width=\"{Helpers.ParseGridLength(column)}\"/>");
                }
                sb.AppendLine("</Grid.ColumnDefinitions>");
            }
            if (gridRows != null && gridRows.Count > 0) {
                sb.AppendLine("<Grid.RowDefinitions>");
                foreach (var row in gridRows) {
                    sb.AppendLine($"<RowDefinition Height=\"{Helpers.ParseGridLength(row)}\"/>");
                }
                sb.AppendLine("</Grid.RowDefinitions>");
            }
            return sb.ToString().CompactString();
        }

    }
}
