using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Mappings;
using Todo_App.Application.Common.Models;
using Todo_App.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Todo_App.Application.TodoLists.Queries.GetTodos;

namespace Todo_App.Application.TodoItems.Queries.FilterTodoItemsByTag;
public record FilterTodoItemsByTagQuery(string Name,int ListId) : IRequest<List<TodoItemDto>>;


public class FilterTodoItemsByTagQueryHandler : IRequestHandler<FilterTodoItemsByTagQuery, List<TodoItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public FilterTodoItemsByTagQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<TodoItemDto>> Handle(FilterTodoItemsByTagQuery request, CancellationToken cancellationToken)
    {
        if (request.Name.ToLower().Equals("all"))
        {
            return (await _context.TodoItems
          .Include(s => s.Tags)
          .Where(s=>s.ListId.Equals(request.ListId) && !s.IsDeleted)
          .ProjectToListAsync<TodoItemDto>(_mapper.ConfigurationProvider));
        }

        return (await _context.TodoItems
            .Include(s => s.Tags)
            .Where(s => s.ListId.Equals(request.ListId) && !s.IsDeleted)
            .Where(x => x.Tags.Any(t => t.Name.Equals(request.Name)))
            .ProjectToListAsync<TodoItemDto>(_mapper.ConfigurationProvider));
    }
}
