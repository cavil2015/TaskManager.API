using Microsoft.EntityFrameworkCore;
using TaskManager.API.Domain;
using TaskManager.API.Infrastructure;

namespace TaskManager.API.Application;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TaskService> _logger;

    public TaskService(AppDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<TaskDto>> GetTasksAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _context.Tasks.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var tasks = await query
            .Include(t => t.TaskType)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TaskDto>(tasks.Select(MapToDto), totalCount, page, pageSize);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .Include(t => t.TaskType)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return task != null ? MapToDto(task) : null;
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            TaskTypeId = dto.TaskTypeId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(task).Reference(t => t.TaskType).LoadAsync(cancellationToken);

        _logger.LogInformation("Создана новая задача: {TaskId} с типом {TaskTypeId}", task.Id, task.TaskTypeId);

        return MapToDto(task);
    }

    public async Task<bool> UpdateTaskAsync(Guid id, UpdateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FindAsync(new object[] { id }, cancellationToken);
        if (task == null) return false;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;
        task.TaskTypeId = dto.TaskTypeId;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Задача {TaskId} успешно обновлена", id);
        return true;
    }

    public async Task<bool> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FindAsync(new object[] { id }, cancellationToken);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Задача {TaskId} удалена", id);
        return true;
    }

    private static TaskDto MapToDto(TaskItem task) =>
        new(task.Id, task.Title, task.Description, task.IsCompleted, task.TaskTypeId, task.TaskType?.Name, task.CreatedAt);
}
