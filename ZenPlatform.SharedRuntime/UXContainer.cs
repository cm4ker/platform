namespace ZenPlatform.SharedRuntime
{
    public class UXContainer
    {
        public UXContainer()
        {
        }

        public UXContainer(string markup, object viewModel)
        {
            Markup = markup;
            ViewModel = viewModel;
        }

        public string Markup { get; set; }

        public object ViewModel { get; set; }
    }
}