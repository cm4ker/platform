using System;
using System.Collections.Generic;
using Avalonia.Controls;
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
        public static UXElement Wrap;
    }

    public abstract class UXElement
    {
        public abstract IControl GetUnderlyingControl();
    }

    public class UXForm : UXElement
    {
        private UserControl _uc;

        public UXForm()
        {
            _uc = new UserControl();
        }

        public void SetContent(UXElement element)
        {
            _uc.conelement.GetUnderlyingControl()
        }

        public override IControl GetUnderlyingControl()
        {
            return _uc;
        }
    }

    public class UXTextBox : UXElement
    {
        private TextBox _c;

        public UXTextBox()
        {
            _c = new TextBox();
        }

        public override IControl GetUnderlyingControl()
        {
            return _c;
        }
    }
}