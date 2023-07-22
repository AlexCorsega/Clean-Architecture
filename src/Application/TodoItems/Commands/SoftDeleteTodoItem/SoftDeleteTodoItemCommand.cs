using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoItems.Commands.SoftDeleteTodoItem;
public record SoftDeleteTodoItemCommand(int Id) : IRequest;
public class SoftDeleteTodoItemCommandHandler : IRequestHandler<SoftDeleteTodoItemCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public SoftDeleteTodoItemCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Unit> Handle(SoftDeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.TodoItems
          .FirstOrDefaultAsync(s => s.Id.Equals(request.Id), cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoList), request.Id);
        }

        entity.IsDeleted = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
