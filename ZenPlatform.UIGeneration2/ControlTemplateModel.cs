namespace ZenPlatform.UIGeneration2 {
    public class ControlTemplateModel<T> : FormComponentModelBase where T : class, IEditEditor, IConstructControlFactory {

        public T EditorProperties { get; set; }

        public ControlTemplateModel(GenerateFormModel generateFormModel, PropertyInformationViewModel viewModel)
            : base(generateFormModel, viewModel) {
            this.EditorProperties = (T)viewModel.ControlSpecificProperties;
        }

    }
}
