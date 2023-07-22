using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Security;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Events;
using ValidationException = Todo_App.Application.Common.Exceptions.ValidationException;

namespace Todo_App.Application.TodoTags.Commands.CreateTag;
//[Authorize]
public record CreateTagCommand(int TodoItemId, string Name) : IRequest<int>;
public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateTagCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await _dbContext.TodoItems
            .AsNoTracking().FirstOrDefaultAsync(s => s.Id.Equals(request.TodoItemId));
        if (todoItem is null)
        {
            throw new NotFoundException("Todo item not found.");
        }
        var tag = await _dbContext.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.TodoItemId.Equals(todoItem.Id) && s.Name.ToLower().Equals(request.Name.ToLower()));
        if(tag is not null)
        {
            throw new ValidationException(new List<ValidationFailure>(){
                new ValidationFailure(nameof(request.Name),"Tag already exist in the todo item.")
            });
        }
        var entity = new Tag
        {
            TodoItemId = request.TodoItemId,
            Name = request.Name,
        };
        entity.AddDomainEvent(new TodoTagCreatedEvent(entity));
        _dbContext.Tags.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
