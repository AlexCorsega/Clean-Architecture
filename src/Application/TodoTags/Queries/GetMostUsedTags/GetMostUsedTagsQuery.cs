using AutoMapper;
using MediatR;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Security;
using Todo_App.Application.TodoTags.Queries.GetUniqueTags;

namespace Todo_App.Application.TodoTags.Queries.GetMostUsedTags;
[Authorize]
public record GetMostUsedTagsQuery : IRequest<List<TagBriefDto>>;
public class GetMostUsedTagsQueryHandler : IRequestHandler<GetMostUsedTagsQuery, List<TagBriefDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _userService;

    public GetMostUsedTagsQueryHandler(IApplicationDbContext dbContext, IMapper mapper,ICurrentUserService userService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userService = userService;
    }
    public async Task<List<TagBriefDto>> Handle(GetMostUsedTagsQuery request, CancellationToken cancellationToken)
    {
        var result = (from s in _dbContext.Tags
                      .Where(s=>s.CreatedBy == _userService.UserId)
                      .GroupBy(s => s.Name)
                      .OrderByDescending(s => s.Count())
                      .Select(s => s.First())
                      select s).AsEnumerable()
                      .Select(t => _mapper.Map<TagBriefDto>(t))
                  .ToList();;

        return result;
    }
}