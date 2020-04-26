namespace ZenPlatform.UI.Ast
{
    /// <summary>
    /// Текстовое поле
    /// </summary>
    public class UITextBox : UINode
    {
        public UITextBox()
        {
            Height = 28;
            Width = 100;
        }

        /// <summary>
        /// Поле для данных
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Высота
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Ширина
        /// </summary>
        public double Width { get; set; }
    }
}