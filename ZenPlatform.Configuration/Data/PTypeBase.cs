namespace ZenPlatform.Configuration.Data
{
    public abstract class PTypeBase
    {
        protected PTypeBase()
        {

        }

        public virtual string Name { get; }

        public virtual string Description { get; set; }
    }
}