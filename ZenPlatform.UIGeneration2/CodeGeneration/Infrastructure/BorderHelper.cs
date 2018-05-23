using System;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure {
    public class BorderHelper {

        readonly String _elementType = "Border";
        readonly ProjectType _projectType;

        public BorderHelper(ProjectType projectType) {
            _projectType = projectType;
            if (projectType == ProjectType.Xamarin) {
                _elementType = "Frame";
            }
        }

        public String EndTag() {
            return $"</{_elementType}>";
        }

        public String StartTag() {
            if (_projectType == ProjectType.Xamarin) {
                return $"<{_elementType} HasShadow=\"True\" OutlineColor=\"Gray\" Padding=\"10\">";
            }
            return $"<{_elementType} BorderThickness=\"1\" BorderBrush=\"Gray\" Padding=\"10\" CornerRadius=\"10\">";
        }

    }
}
