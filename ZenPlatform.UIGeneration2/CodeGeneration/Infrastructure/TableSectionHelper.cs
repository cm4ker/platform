using System;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure {
    public class TableSectionHelper {

        public TableSectionHelper() {
        }

        public String EndTag() {
            return "</TableSection>";
        }

        public String StartTag(String title) {
            return $"<TableSection Title=\"{title}\">";
        }

    }
}
