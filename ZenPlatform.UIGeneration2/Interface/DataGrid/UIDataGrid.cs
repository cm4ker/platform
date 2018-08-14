using System;

namespace ZenPlatform.UIBuilder.Interface.DataGrid
{
    public class UIDataGrid : UINode
    {
        public UIDataGrid()
        {
        }

        public UIDataGrid WithColumn(Func<UIColumnFactory, UIDataGridColumn> columnExpr)
        {
            Add(columnExpr(UIColumnFactory.Get()));

            return this;
        }
    }

    public class UIDataGridColumn : UINode
    {
    }


    public class UIColumnFactory
    {
        private static UIColumnFactory _instance = new UIColumnFactory();

        /// <summary>
        ///  Получить фабрику
        /// </summary>
        /// <returns></returns>
        public static UIColumnFactory Get()
        {
            return _instance;
        }


        public UIDataGridColumn Column()
        {
            return new UIDataGridColumn();
        }
    }
}