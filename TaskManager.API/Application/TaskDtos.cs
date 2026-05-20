namespace TaskManager.API.Application;

public record TaskDto(Guid Id, string Title, string? Description, bool IsCompleted, Guid TaskTypeId, string? TaskTypeName, DateTime CreatedAt);
public record CreateTaskDto(string Title, string? Description, Guid TaskTypeId);
public record UpdateTaskDto(string Title, string? Description, bool IsCompleted, Guid TaskTypeId);
public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);
