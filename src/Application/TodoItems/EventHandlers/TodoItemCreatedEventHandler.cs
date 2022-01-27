using CleanArchitecture.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.TodoItems.EventHandlers;

public class TodoItemCreatedEventConsumer : IConsumer<TodoItemCreatedEvent>
{
    private readonly ILogger<TodoItemCreatedEventConsumer> _logger;

    public TodoItemCreatedEventConsumer(ILogger<TodoItemCreatedEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<TodoItemCreatedEvent> context)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", context.Message.GetType().Name);

        return Task.CompletedTask;
    }
}