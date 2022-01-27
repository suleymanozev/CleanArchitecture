namespace CleanArchitecture.Domain.Events;

public class TodoItemDeletedEvent : DomainEvent
{
    public TodoItemDeletedEvent(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
