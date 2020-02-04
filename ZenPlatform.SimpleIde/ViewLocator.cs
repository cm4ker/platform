using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dock.Model;
using ReactiveUI;
using ZenPlatform.SimpleIde.ViewModels;
using System.Linq;
using ZenPlatform.Ide.Contracts;

namespace ZenPlatform.SimpleIde
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            Type  type = null;
            var attrs = data.GetType().GetCustomAttributes(typeof(ViewAttribute), true);
            if (attrs.Length > 0)
            {
                type = ((ViewAttribute)attrs.First()).ViewType;
            }
            else
            {
                var name = data.GetType().FullName.Replace("ViewModel", "View");
                type = Type.GetType(name);
            }

            if (type != null)
            {
                /*
                if (type.GetConstructor(new Type[]{ data.GetType() }) != null)
                {

                }*/
                var view = (Control)Activator.CreateInstance(type);
              
                return view;
            }
            else
            {
                return new TextBlock { Text = "View not found for : " + data.GetType().FullName };
            }
        }

        public bool Match(object data)
        {
            return data is ReactiveObject || data is IDockable;
        }
    }


    public class ConfigurationItemViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object param)
        {
            throw new NotImplementedException();
        }

        public bool Match(object data)
        {
            return data is IConfigurationItem;
        }
    }
}