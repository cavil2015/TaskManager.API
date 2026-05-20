using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.API.Application;
using TaskManager.API.Domain;
using TaskManager.API.Infrastructure;
using Xunit;

namespace TaskManager.Tests;

public class TaskServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TaskService _taskService;
    private readonly Mock<ILogger<TaskService>> _loggerMock;

    public TaskServiceTests()
    {
        // Настраиваем InMemory базу данных для тестирования EF Core без реального Postgres
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя для каждого запуска
            .Options;

        _context = new AppDbContext(options);
        
        // Мокаем логгер, так как нам не нужно реально писать логи в консоль во время тестов
        _loggerMock = new Mock<ILogger<TaskService>>();

        _taskService = new TaskService(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldCreateTask_AndReturnDto()
    {
        // Arrange (Подготовка)
        var taskTypeId = Guid.NewGuid();
        var type = new TaskType { Id = taskTypeId, Name = "Bug" };
        _context.TaskTypes.Add(type);
        await _context.SaveChangesAsync();

        var dto = new CreateTaskDto("Test Task", "Description", taskTypeId);

        // Act (Действие)
        var result = await _taskService.CreateTaskAsync(dto);

        // Assert (Проверка)
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal("Bug", result.TaskTypeName); // Проверяем, что навигационное свойство подгрузилось
        
        var taskInDb = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == result.Id);
        Assert.NotNull(taskInDb);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _taskService.GetTaskByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnTrue_WhenTaskDeleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem { Id = taskId, Title = "To Delete", TaskTypeId = Guid.NewGuid() };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.DeleteTaskAsync(taskId);

        // Assert
        Assert.True(result);
        var taskInDb = await _context.Tasks.FindAsync(taskId);
        Assert.Null(taskInDb);
    }

    // Очистка ресурсов после каждого теста
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
