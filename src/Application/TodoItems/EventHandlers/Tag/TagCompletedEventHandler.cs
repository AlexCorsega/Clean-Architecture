using MediatR;
using Microsoft.Extensions.Logging;
using Todo_App.Domain.Events;

namespace Todo_App.Application.TodoItems.EventHandlers;

public class TagCompletedEventHandler : INotificationHandler<TodoTagCompletedEvent>
{
    private readonly ILogger<TagCompletedEventHandler> _logger;

    public TagCompletedEventHandler(ILogger<TagCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoTagCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Todo_App Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
