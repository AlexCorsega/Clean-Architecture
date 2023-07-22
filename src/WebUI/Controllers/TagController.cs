using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.TodoTags.Commands.CreateTag;
using Todo_App.Application.TodoTags.Commands.DeleteTag;

namespace Todo_App.WebUI.Controllers;
public class TagController : ApiControllerBase
{
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
