using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoForSso.TodoApi.Application.Dtos;
using TodoForSso.TodoApi.Application.Services;

namespace TodoForSso.TodoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/todos")]
public class TodosController(ITodoService todoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TodoDto>>> GetAll() => Ok(await todoService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoDto>> GetById(Guid id)
    {
        var todo = await todoService.GetByIdAsync(id);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<TodoDto>> Create(CreateTodoDto dto)
    {
        var todo = await todoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TodoDto>> Update(Guid id, UpdateTodoDto dto)
    {
        var todo = await todoService.UpdateAsync(id, dto);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await todoService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<TodoDto>> Complete(Guid id)
    {
        var todo = await todoService.CompleteAsync(id);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpPatch("{id:guid}/reopen")]
    public async Task<ActionResult<TodoDto>> Reopen(Guid id)
    {
        var todo = await todoService.ReopenAsync(id);
        return todo is null ? NotFound() : Ok(todo);
    }
}
