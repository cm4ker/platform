namespace ZenPlatform.DataComponent
{
    /*
        * Необходимо соблюдать формат возвращаемых шаблонов для генерации инструкций
        * Например
        * 
        * Для докумета необходимо, чтобы в чужих свойствах передавался идентификатор для того, чтобы можно было загрузить документ
        * Инструкция должна выглядить следующим образом
        * 
        * Session.{ComponentName}.{ObjectName}.Load({Params}) => Session.Document.Invoice.Load(_dto.InvoiceKey)
        * 
        */


    public class StandartGetExpressionParameters
    {
        /// <summary>
        /// Имя компонента
        /// </summary>
        public string ComponentName { get; set; }
        
        /// <summary>
        /// Имя объекта
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Перечисление параметров через запятую!
        /// </summary>
        public string Params { get; set; }
    }

    public class StandartSetExpressionParameters
    {
        /// <summary>
        /// Куда необходимо установить значение
        /// </summary>
        public string SetVariable { get; set; }

        /// <summary>
        /// Имя компонента
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// Имя объекта
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Перечисление параметров через запятую!
        /// </summary>
        public string Params { get; set; }
    }
}