using Microsoft.EntityFrameworkCore;
using TaskManager.API.Application;
using TaskManager.API.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;

// --- Инициализация Serilog (bootstrap logger до построения хоста) ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Запуск приложения TaskManager API");

    var builder = WebApplication.CreateBuilder(args);

    // Подключаем Serilog к конвейеру логирования .NET
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // MVC + Swagger
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Валидация
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskDtoValidator>();

    // Глобальная обработка ошибок
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // База данных
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Сервисы приложения
    builder.Services.AddScoped<ITaskService, TaskService>();

    var app = builder.Build();

    // Логирование HTTP-запросов
    app.UseSerilogRequestLogging();

    // Глобальная обработка исключений
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();
    app.MapControllers();

    // Автоматическое применение миграций при старте
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
            Log.Information("Миграции базы данных успешно применены.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Произошла ошибка при применении миграций к БД.");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение неожиданно завершило работу");
}
finally
{
    Log.CloseAndFlush();
}
