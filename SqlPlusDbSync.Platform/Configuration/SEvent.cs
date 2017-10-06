namespace SqlPlusDbSync.Platform.Configuration
{
    public class SEvent
    {
        public SEvent()
        {

        }

        public string Name { get; set; }

        public EventType EventType { get; set; }

        public string Body { get; set; }
    }
}