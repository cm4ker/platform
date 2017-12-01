using System;

namespace ZenPlatform.UIGeneration2 {
    [Serializable]
    public class PropertyParameter {

        public String ParameterName { get; set; }

        public String ParameterTypeName { get; set; }

        public PropertyParameter() {
        }

        public PropertyParameter(String parameterName, String parameterTypeName) {
            if (String.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(parameterName));
            if (String.IsNullOrWhiteSpace(parameterTypeName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(parameterTypeName));

            this.ParameterName = parameterName;
            this.ParameterTypeName = parameterTypeName;
        }

    }
}
