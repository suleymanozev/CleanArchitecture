using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services;

public class DomainEventService : IDomainEventService
{
    private readonly ILogger<DomainEventService> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public DomainEventService(ILogger<DomainEventService> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Publish(DomainEvent domainEvent)
    {
        _logger.LogInformation("Publishing domain event. Event - {Event}", domainEvent.GetType().Name);
        await _publishEndpoint.Publish(domainEvent, domainEvent.GetType());
    }
}