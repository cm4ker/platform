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
        public PEvent(PEventType eventType, string Name)
        {

        }

        public string Name { get; set; }

        public PEventType EventType { get; set; }

        public string Module { get; set; }
    }
}