using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.Common.Security;
using Todo_App.Application.TodoTags.Commands.CreateTag;
using Todo_App.Application.TodoTags.Commands.DeleteTag;
using Todo_App.Application.TodoTags.Queries.GetMostUsedTags;
using Todo_App.Application.TodoTags.Queries.GetUniqueTags;

namespace Todo_App.WebUI.Controllers;
public class TagController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TagBriefDto>>> GetUniqueTags([FromQuery]GetUniqueTagsQuery query)
    {
        return await Mediator.Send(query);
    }
    [HttpGet("[action]")]
    public async Task<ActionResult<List<TagBriefDto>>> GetMostUsedTags()
    {
        return await Mediator.Send(new GetMostUsedTagsQuery());
    }
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTagCommand command)
    {
        var result = await Mediator.Send(command);
        return result;
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTagCommand(id));

        return NoContent();
    }
}
