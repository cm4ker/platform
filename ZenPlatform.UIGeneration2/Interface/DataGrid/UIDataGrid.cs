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
        public string DataSource { get; set; }
    }

    public class UIDataGridTemplatedColumn : UINode
    {
        public UIDataGridTemplatedColumn With(Func<UIFactory, UINode> columnExpr)
        {
            Add(columnExpr(UIFactory.Get()));

            return this;
        }
    }

    public class UIDataGridTextColumn : UIDataGridColumn
    {

    }

    public class UIDataGridObjectColumn : UIDataGridColumn
    {

    }

    public class UIDataGridDateColumn : UIDataGridColumn
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


        public UIDataGridTextColumn TextColumn()
        {
            return new UIDataGridTextColumn();
        }

        public UIDataGridColumn UIDataGridAnyTypeColumn()
        {
            return new UIDataGridColumn();
        }


        public UIDataGridObjectColumn ObjectColumn()
        {
            return new UIDataGridObjectColumn();
        }
    }
}