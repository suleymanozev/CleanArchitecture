namespace CleanArchitecture.Domain.Events;

public class TodoItemCreatedEvent : DomainEvent
{
    public TodoItemCreatedEvent(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
