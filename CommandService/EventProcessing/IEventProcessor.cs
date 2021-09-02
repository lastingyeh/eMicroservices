namespace commandservice.EventProcessing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}