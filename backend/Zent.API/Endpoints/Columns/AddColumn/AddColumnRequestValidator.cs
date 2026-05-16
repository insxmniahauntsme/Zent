using FluentValidation;

namespace Zent.API.Endpoints.Columns.AddColumn;

public sealed class AddColumnRequestValidator : AbstractValidator<AddColumnRequest>
{
    public AddColumnRequestValidator()
    {
        RuleFor(x => x.Title)
            .Must(title =>
                string.IsNullOrWhiteSpace(title) ||
                title.Trim().Length >= 2
            )
            .WithMessage("Title must be at least 2 characters long.");

        RuleFor(x => x.Title)
            .Must(title =>
                string.IsNullOrWhiteSpace(title) ||
                title.Trim().Length <= 32
            )
            .WithMessage("Title must be no longer than 32 characters.");
    }
}