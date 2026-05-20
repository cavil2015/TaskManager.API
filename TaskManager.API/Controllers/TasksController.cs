using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Application;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _taskService.GetTasksAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _taskService.GetTaskByIdAsync(id, cancellationToken);
        if (task == null) return NotFound("Задача не найдена.");

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var createdTask = await _taskService.CreateTaskAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var success = await _taskService.UpdateTaskAsync(id, dto, cancellationToken);
        if (!success) return NotFound("Задача не найдена.");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var success = await _taskService.DeleteTaskAsync(id, cancellationToken);
        if (!success) return NotFound("Задача не найдена.");

        return NoContent();
    }
}
