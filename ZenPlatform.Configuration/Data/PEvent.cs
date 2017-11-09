namespace ZenPlatform.Configuration.Data
{
    public enum PEventType
    {
        AfterLoad,
        BeforeSave,
        AfterSave
    }

    public class PEvent
    {
        public PEvent(PEventType eventType, string name)
        {
            EventType = eventType;
            Name = name;
        }

        public string Name { get; set; }

        public PEventType EventType { get; set; }

        public string Code { get; set; }
    }
}