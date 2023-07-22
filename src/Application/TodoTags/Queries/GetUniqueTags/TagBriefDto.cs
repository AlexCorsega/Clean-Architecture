using AutoMapper;
using Todo_App.Application.Common.Mappings;
using Todo_App.Application.TodoLists.Queries.GetTodos;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoTags.Queries.GetUniqueTags;
public  class TagBriefDto : IMapFrom<Tag>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    //public void Mapping(Profile profile)
    //{
    //    profile.CreateMap<TagBriefDto, Tag>()
    //        .ForMember(t => t.Id, opt => opt.MapFrom(s => s.Id))
    //        .ForMember(t => t.Name, opt => opt.MapFrom(s => s.Name));
    //    profile.CreateMap<Tag, TagBriefDto>();
    //}
}
