namespace Todo_App.Domain.Events;
public class TodoTagCreatedEvent : BaseEvent
{
    public TodoTagCreatedEvent(Tag tag)
    {

        Tag = tag;

    }
    public Tag Tag { get; set; }
}
