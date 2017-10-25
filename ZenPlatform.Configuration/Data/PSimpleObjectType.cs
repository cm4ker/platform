namespace ZenPlatform.Configuration.Data
{
    /// <summary>
    /// Простой объект с полями. 
    /// </summary>
    public class PSimpleObjectType : PObjectType
    {
        public PSimpleObjectType(string name) : base(name)
        {

        }

        public override bool IsAbstractType => false;
    }
}