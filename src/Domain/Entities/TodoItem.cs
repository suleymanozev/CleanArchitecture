namespace CleanArchitecture.Domain.Entities;

public class TodoItem : AuditableEntity, IHasDomainEvent
{
    public Guid ListId { get; set; }

    public string? Title { get; set; }

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value && _done == false)
            {
                DomainEvents.Add(new TodoItemCompletedEvent(Id));
            }

            _done = value;
        }
    }

    public virtual TodoList List { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; set; } = new();
}
