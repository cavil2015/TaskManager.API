using Microsoft.EntityFrameworkCore;
using TaskManager.API.Domain;

namespace TaskManager.API.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks { get; set; } = null!;
    public DbSet<TaskType> TaskTypes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка связей
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.TaskType)
            .WithMany(tt => tt.Tasks)
            .HasForeignKey(t => t.TaskTypeId)
            .OnDelete(DeleteBehavior.Restrict); // Запрещаем удалять тип, если есть связанные задачи

        // Предзаполнение базы (Seeding)
        var bugType = new TaskType { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Bug (Ошибка)" };
        var featureType = new TaskType { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Feature (Новая функция)" };
        var maintenanceType = new TaskType { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Maintenance (Обслуживание)" };

        modelBuilder.Entity<TaskType>().HasData(bugType, featureType, maintenanceType);
    }
}
