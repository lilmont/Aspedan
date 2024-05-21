namespace Aspedan.Entities.Validation;

public class TaskItemValidation : AbstractValidator<TaskItemDto>
{
	public TaskItemValidation()
	{
		RuleFor(p => p.Title)
			.NotNull().WithMessage("Title cannot be null!")
			.NotEmpty().WithMessage("Title cannot be empty!")
			.MaximumLength(50).WithMessage("Title cannot exceed 50 characters!");

		RuleFor(p => p.Description)
			.NotNull().WithMessage("Description cannot be null!")
			.NotEmpty().WithMessage("Description cannot be empty!")
			.MaximumLength(500).WithMessage("Description cannot exceed 500 characters!");

		RuleFor(p => p.DueDate)
			.NotNull().WithMessage("DueDate cannot be null!")
			.NotEmpty().WithMessage("DueDate cannot be empty!");
	}
}
