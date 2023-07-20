using MediatR;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Security;
using Todo_App.Domain.Entities;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.TodoItems.Commands.UpdateBackgroundTodoItem;
[Authorize]
public record UpdateBackgroundTodoItemCommand(int Id,string Colour) : IRequest;
public class UpdateBackgroundTodoItemCommandHandler : IRequestHandler<UpdateBackgroundTodoItemCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateBackgroundTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Unit> Handle(UpdateBackgroundTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems.FindAsync(request.Id);
        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }
        entity.BackgroundColour = Colour.From(request.Colour);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
