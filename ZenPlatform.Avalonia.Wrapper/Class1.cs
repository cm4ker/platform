using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.Styling;

namespace ZenPlatform.Avalonia.Wrapper
{
    public interface IUIElement
    {
        string Name { get; }
    }

    public interface IContainer
    {
        ElementAlignmentType ElementsAlignment { get; set; }

        void SetContent(IUIElement element);

        IUIElement Content { get; }
    }

    public interface IColumn
    {
        int Index();

        int Width();
    }

    public interface IRow
    {
        int Index();

        int Height();
    }

    public interface IGrid : IUIElement
    {
        IColumn CreateColumn();
        IRow CreateRow();

        IContainer GetContainer(int row, int column);
        IContainer GetContainer(IRow row, IColumn column);
    }

    public interface ILabel : IUIElement
    {
        string Text { get; set; }
    }

    public enum ElementAlignmentType
    {
        Vertical,
        Horizontal
    }

    public interface IWindow : IContainer
    {
    }

    public class ZWindow : IWindow
    {
        private Window _w;

        public ZWindow()
        {
            _w = new Window();
        }

        public ElementAlignmentType ElementsAlignment { get; set; }

        public void SetContent(IUIElement element)
        {
            throw new NotImplementedException();
        }

        public IUIElement Content { get; }
    }

    /*
     * {
     *     if(User.HasRole("Admin"))
     *         this.MyFavoriteCheckbox.Enable = true;
     *     else
     *         this.MyFavoriteCheckbox.Enable = false;
     *                     ^
     *             Это свойство формы
     * }
     * 
     */
}