using System;

namespace ZenPlatform.UIGeneration2.CodeGeneration {
    public class UIGeneration {

        /// <summary>
        /// Generates the specified XAML UI from the data.
        /// </summary>
        /// <param name="generateFormModel">The code generation data.</param>
        /// <returns>String of XAML</returns>
        /// <exception cref="InvalidOperationException">$Unable to complete code generation: {formTemplate.Errors}</exception>
        public String Generate(GenerateFormModel generateFormModel) {
            generateFormModel.PreGenerateConfiguration();
            var formTemplate = new UIFormTemplate(generateFormModel);
            var xaml = formTemplate.TransformText();
            
            return xaml;
        }

    }
}
