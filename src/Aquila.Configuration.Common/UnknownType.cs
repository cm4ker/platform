namespace Aquila.Configuration.Common
{
    public class MDPrimitive : MDType
    {
    }

    /// <summary>
    /// Неопределённый тип, при загрузке конфигурации сначала всё приводится к нему
    /// </summary>
    public class UnknownType : MDType
    {
        protected override bool ShouldSerializeDescription()
        {
            return false;
        }

        protected override bool ShouldSerializeName()
        {
            return false;
        }

        protected override bool ShouldSerializeId()
        {
            return false;
        }
    }
}