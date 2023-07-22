using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Security;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoTags.Commands.DeleteTag;
[Authorize]
public record DeleteTagCommand(int Id) : IRequest;
class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteTagCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public  async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id.Equals(request.Id),cancellationToken);
        if(entity is null)
        {
            throw new NotFoundException(nameof(Tag),request.Id);
        }
        _dbContext.Tags.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
