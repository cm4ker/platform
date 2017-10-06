namespace QMSSystem
{
    public class SubscribeParameters
    {
        public SubscribeParameters(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool Durable { get; set; }
        public bool AutoRemove { get; set; }
        public bool Exclusive { get; set; }
    }
}