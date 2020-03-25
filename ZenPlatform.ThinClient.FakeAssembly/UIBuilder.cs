using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ZenPlatform.ClientRuntime;

namespace ZenPlatform.ThinClient.FakeAssembly
{
    public class UX
    {
        public static UXContainer GetDesktop()
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


            return new UXContainer {Markup = xaml, ViewModel = null};
        }

        public static IControl GetDocumentForm()
        {
            return null;
        }
    }

/*
 
 */
/*
 Instance call
 
    Client                        Server
                                    
        
 OnClientMethod             OnServerMethodCall
       |                           ^
       V         ObjectPack        |
 OnServerMethod -------------> UnpackObject 
                        
 
 */
}