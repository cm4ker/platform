namespace ZenPlatform.Core.Network
{
    public interface ITaskManager
    {
        void FinishTask(InvokeContext invokeContext);
        void StartTask(InvokeContext invokeContext);
    }
}