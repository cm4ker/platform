namespace ZenPlatform.UIGeneration2 {
    public interface IConstructControlFactory {

        IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel);

    }
}