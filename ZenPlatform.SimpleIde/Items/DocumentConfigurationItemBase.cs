using Avalonia.Media.Imaging;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using Dock.Model.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.SimpleIde.ViewModels;
using ZenPlatform.SimpleIde.Views;

namespace ZenPlatform.SimpleIde
{
    
    class DocumentConfigurationItemBase : Document, IConfiguratoinItem
    {
        public string Content => Title;

        public IBitmap Bitmap => null;

        public bool IsEnable => true;

        public bool IsExpanded => false;

        public bool CanOpen => true;

        public bool HasContext => true;

        public IEnumerable<IConfiguratoinItem> Childs => null;



    }


    

}
