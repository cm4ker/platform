using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZenPlatform.ThinClient.FakeAssembly
{
    public class UIBuilder
    {
        public static string GetDesktop()
        {
            //Preapre: Get XAML from the server + invoke server logic
            var xaml = @"
<UXForm xmlns=""clr-namespace:ZenPlatform.Avalonia.Wrapper;assembly=ZenPlatform.Avalonia.Wrapper"">
  <UXGroup Orientation=""Horizontal"">
    <UXTextBox />
    <UXTextBox />
    <UXTextBox />
  </UXGroup>
</UXForm>";

            return xaml;
        }

        public static IControl GetDocumentForm()
        {
            return null;
        }
    }
}