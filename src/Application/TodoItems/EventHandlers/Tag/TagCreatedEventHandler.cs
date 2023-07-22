using MediatR;
using Microsoft.Extensions.Logging;
using Todo_App.Domain.Events;

namespace Todo_App.Application.TodoItems.EventHandlers;

public class TagCreatedEventHandler : INotificationHandler<TodoTagCreatedEvent>
{
    private readonly ILogger<TagCreatedEventHandler> _logger;

    public TagCreatedEventHandler(ILogger<TagCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoTagCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Todo_App Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}