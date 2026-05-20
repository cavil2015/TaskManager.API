namespace TaskManager.API.Application;

public interface ITaskService
{
    Task<PagedResult<TaskDto>> GetTasksAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<TaskDto?> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateTaskAsync(Guid id, UpdateTaskDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default);
}
