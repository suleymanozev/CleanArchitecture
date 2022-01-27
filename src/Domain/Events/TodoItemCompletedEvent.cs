namespace CleanArchitecture.Domain.Events;

public class TodoItemCompletedEvent : DomainEvent
{
    public TodoItemCompletedEvent(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
