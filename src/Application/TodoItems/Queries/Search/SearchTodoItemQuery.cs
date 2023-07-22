using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Mappings;
using Todo_App.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Todo_App.Application.TodoLists.Queries.GetTodos;

namespace Todo_App.Application.TodoItems.Queries.Search;
public record SearchTodoItemQuery(string? Title,int ListId) : IRequest<List<TodoItemDto>>;
class SearchTodoItemQueryHandler : IRequestHandler<SearchTodoItemQuery, List<TodoItemDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public SearchTodoItemQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<List<TodoItemDto>> Handle(SearchTodoItemQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return await _dbContext.TodoItems
             .Where(x => !x.IsDeleted && x.ListId.Equals(request.ListId))
             .ProjectToListAsync<TodoItemDto>(_mapper.ConfigurationProvider);
        }
        return await _dbContext.TodoItems
             .Where(x => x.Title.ToLower().StartsWith(request.Title.ToLower()) && !x.IsDeleted)
             .ProjectToListAsync<TodoItemDto>(_mapper.ConfigurationProvider); 
    }
}

