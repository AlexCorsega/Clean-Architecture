namespace Todo_App.Domain.Events;

public class TodoTagCompletedEvent : BaseEvent
{
    public TodoTagCompletedEvent(Tag item)
    {
        Item = item;
    }

    public Tag Item { get; }
}
