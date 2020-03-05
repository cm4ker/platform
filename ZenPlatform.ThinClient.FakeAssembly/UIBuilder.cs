using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZenPlatform.ThinClient.FakeAssembly
{
    public class UIBuilder
    {
        public static IControl GetDesktop()
        {
            //Preapre: Get XAML from the server + invoke server logic
            var xaml = @"
<UserControl xmlns='https://github.com/avaloniaui'
             xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
             Width='300'>
    <StackPanel> 
        <TextBlock Text='Store caption' />
        <StackPanel Orientation='Horizontal'>
            <TextBlock Text='Prop1'/>
            <TextBox Text='test' Width='200'/>
        </StackPanel>
        <StackPanel Orientation='Horizontal'>
            <TextBlock Text='Prop2'/>
            <TextBox Text='test' Width='200'/>
        </StackPanel>
    </StackPanel>
</UserControl>";
            return (IControl) AvaloniaXamlLoader.Parse(xaml);
        }

        public static IControl GetDocumentForm()
        {
            return null;
        }
    }
}