using System.Runtime.InteropServices.WindowsRuntime;
using Avalonia.Controls;

namespace ZenPlatform.Avalonia.Wrapper.ControlLayer
{
    public class UserControlWrapper : IControlWrapper
    {
        private UserControl _control;
        private object _content;

        public UserControlWrapper()
        {
            _control = new UserControl();
        }

        public object Content
        {
            get => _control.Content;
            set => _control.Content = value;
        }

        public object GetUnderlyingControl()
        {
            return _control;
        }
    }

    public class GridWrapper : IControlWrapper
    {
        private Grid _control;

        public GridWrapper()
        {
            _control = new Grid();
        }


        public void AddChild(IControlWrapper item)
        {
            _control.Children.Add((Control) item.GetUnderlyingControl());
        }

        public void RemoveChild(IControlWrapper item)
        {
            _control.Children.Remove((Control) item.GetUnderlyingControl());
        }

        public object GetUnderlyingControl()
        {
            return _control;
        }
    }

    public class FakeUserControlWrapper : IControlWrapper
    {
        public object GetUnderlyingControl()
        {
            throw new System.NotImplementedException();
        }
    }


    public interface IControlWrapper
    {
        object GetUnderlyingControl();
    }

    public class ControlsFactory
    {
        public UserControlWrapper CreateUserControl()
        {
            return new UserControlWrapper();
        }

        public GridWrapper CreateGridControl()
        {
            return new GridWrapper();
        }
    }
}