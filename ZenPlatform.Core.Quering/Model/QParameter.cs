namespace ZenPlatform.Core.Quering.Model
{
    /// <summary>
    /// Представляет параметр
    /// </summary>
    public class QParameter : QExpression
    {
        public string Name { get; }

        public QParameter(string name)
        {
            Name = name;
        }
    }
}