using FluentValidation;

namespace TaskManager.API.Application;

// Правила валидации для создания задачи
public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название задачи не может быть пустым.")
            .MaximumLength(100).WithMessage("Название задачи не должно превышать 100 символов.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов.");

        RuleFor(x => x.TaskTypeId)
            .NotEmpty().WithMessage("Необходимо указать тип задачи.");
    }
}

// Правила валидации для обновления задачи
public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название задачи не может быть пустым.")
            .MaximumLength(100).WithMessage("Название задачи не должно превышать 100 символов.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов.");

        RuleFor(x => x.TaskTypeId)
            .NotEmpty().WithMessage("Необходимо указать тип задачи.");
    }
}
