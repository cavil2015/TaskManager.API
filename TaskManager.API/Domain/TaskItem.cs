namespace TaskManager.API.Domain;

// Основная сущность "Задача"
public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }

    // Внешний ключ на тип задачи
    public Guid TaskTypeId { get; set; }
    public TaskType? TaskType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
