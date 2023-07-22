using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.TodoItems.Commands.SoftDeleteTodoItem;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Application.TodoLists.Commands.DeleteTodoList;
using Todo_App.Application.TodoLists.Commands.SoftDeleteTodoList;
using Todo_App.Application.TodoLists.Commands.UpdateTodoList;
using Todo_App.Application.TodoLists.Queries.ExportTodos;
using Todo_App.Application.TodoLists.Queries.GetTodos;

namespace Todo_App.WebUI.Controllers;

public class TodoListsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TodosVm>> Get()
    {
        var result = await Mediator.Send(new GetTodosQuery());
        return result;
    }

    [HttpGet("{id}")]
    public async Task<FileResult> Get(int id)
    {
        var vm = await Mediator.Send(new ExportTodosQuery { ListId = id });

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoListCommand command)
    {
        var result = await Mediator.Send(command);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTodoListCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        //actual Delete
        //await Mediator.Send(new DeleteTodoListCommand(id));

        //implementation of soft delete
        await Mediator.Send(new SoftDeleteTodoListCommand(id));

        return NoContent();
    }
}
