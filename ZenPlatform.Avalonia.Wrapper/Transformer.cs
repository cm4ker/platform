using System.Net.Sockets;
using System.Xml;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.OpenGL;
using Avalonia.Styling;
using JetBrains.Annotations;

namespace ZenPlatform.Avalonia.Wrapper
{
    /*
        Transformer.Wrap(string) => object
        Transformer.Unwrap(UIElement) => string
    */

    public static class Transformer
    {
        public static UXElement Wrap()
        {
            var form = new UXForm();
            var tb = new UXTextBox();

                        return form;
        }
    }

    /*
      <UXForm> 
         <UXGroup Orient="Horizontal">
             <UXTextBox />
             <UXTextBox />
         </UXGroup> 
      </UXForm>
      */
}