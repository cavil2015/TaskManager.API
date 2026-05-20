namespace TaskManager.API.Domain;

// Отдельная сущность "Тип задачи"
public class TaskType
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    // Навигационное свойство
    public List<TaskItem> Tasks { get; set; } = new();
}
