namespace ZenPlatform.Configuration.Data
{
    /// <summary>
    /// ������� ������ � ������. 
    /// </summary>
    public class PSimpleObjectType : PObjectType
    {
        public PSimpleObjectType(string name) : base(name)
        {

        }

        public override bool IsAbstractType => false;
    }
}