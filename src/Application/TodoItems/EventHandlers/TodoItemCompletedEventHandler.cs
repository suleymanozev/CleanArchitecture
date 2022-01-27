using CleanArchitecture.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.TodoItems.EventHandlers;

public class TodoItemCompletedEventConsumer : IConsumer<TodoItemCompletedEvent>
{
    private readonly ILogger<TodoItemCompletedEventConsumer> _logger;

    public TodoItemCompletedEventConsumer(ILogger<TodoItemCompletedEventConsumer> logger)
    {
        _logger = logger;
    }
    public Task Consume(ConsumeContext<TodoItemCompletedEvent> context)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", context.Message.GetType().Name);
        return Task.CompletedTask;
    }
}
