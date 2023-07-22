using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Mappings;
using Todo_App.Application.Common.Security;

namespace Todo_App.Application.TodoTags.Queries.GetUniqueTags;
[Authorize]
public record GetUniqueTagsQuery(int TodoListId) : IRequest<List<TagBriefDto>>;
public class GetUniqueTagsQueryHandler : IRequestHandler<GetUniqueTagsQuery, List<TagBriefDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetUniqueTagsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<List<TagBriefDto>> Handle(GetUniqueTagsQuery request, CancellationToken cancellationToken)
    {
        var result = (from s in _dbContext.Tags
                 .Where(t => t.TodoItem.ListId.Equals(request.TodoListId) && !t.TodoItem.IsDeleted)
                    select s).GroupBy(t => t.Name).Select(t => _mapper.Map<TagBriefDto>(t.First()))
                    .ToList();
        return result;
    }
}
