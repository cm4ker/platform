namespace ZenPlatform.UIGeneration2 {
    public class FormComponentModel : FormComponentModelBase {

        public IControlFactory ControlFactory { get; }

        public FormComponentModel(GenerateFormModel generateFormModel, PropertyInformationViewModel viewModel)
            : base(generateFormModel, viewModel) {
            this.ControlFactory = viewModel.GetTemplateFactory().Make(generateFormModel, viewModel);
        }

    }
}
