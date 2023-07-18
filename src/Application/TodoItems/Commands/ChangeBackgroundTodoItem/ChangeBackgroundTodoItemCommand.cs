using MediatR;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.TodoItems.Commands.ChangeBackgroundTodoItem;
public record ChangeBackgroundTodoItemCommand(int TodoItemId, string BackgroundColour) : IRequest;
public class ChangeBackgroundTodoItemCommandHandler : IRequestHandler<ChangeBackgroundTodoItemCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public ChangeBackgroundTodoItemCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
   public async Task<Unit> Handle(ChangeBackgroundTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.TodoItems.FindAsync(request.TodoItemId);
        entity.BackgroundColour = Colour.From(request.BackgroundColour);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

