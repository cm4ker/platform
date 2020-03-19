using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dock.Model;
using ReactiveUI;
using ZenPlatform.ThinClient.ViewModels;

namespace ZenPlatform.ThinClient
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            if (data is DocumentView dd)
            {
                data = dd.Context;
            }

            if (data is RuntimeModel rm)
                return rm.Run();

            Type type = null;
            
            {
                var name = data?.GetType().FullName?.Replace("ViewModel", "View");

                if (name != null)
                    type = Type.GetType(name);
            }

            if (type != null)
            {
                var view = (Control) Activator.CreateInstance(type);
                return view;
            }
            else
            {
                return new TextBlock {Text = "View not found for : " + data?.GetType().FullName};
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
            return true;
        }
    }
}