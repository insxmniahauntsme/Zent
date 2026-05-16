using FluentValidation;

namespace Zent.API.Endpoints.Tasks.AddTask;

public sealed class AddTaskRequestValidator : AbstractValidator<AddTaskRequest>
{
    public AddTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Title is required.")
            .Must(title => title.Trim().Length is >= 2 and <= 64)
            .WithMessage("Title must be between 2 and 64 characters.");

        RuleFor(x => x.Description)
            .Must(description =>
                string.IsNullOrWhiteSpace(description) ||
                description.Trim().Length <= 128)
            .WithMessage("Description must be less than 128 characters.");

        RuleFor(x => x.Priority).IsInEnum();

        RuleFor(x => x.UntilDate)
            .Must(untilDate =>
                !untilDate.HasValue ||
                untilDate.Value >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Until date cannot be in the past.");
    }
}