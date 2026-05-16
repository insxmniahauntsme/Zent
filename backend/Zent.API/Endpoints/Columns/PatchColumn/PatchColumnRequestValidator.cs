using FluentValidation;
using Zent.API.Endpoints.Columns.AddColumn;

namespace Zent.API.Endpoints.Columns.PatchColumn;

public sealed class PatchColumnRequestValidator : AbstractValidator<PatchColumnRequest>
{
    public PatchColumnRequestValidator()
    {
        RuleFor(x => x.Title)
            .Must(title =>
                string.IsNullOrWhiteSpace(title) ||
                title.Trim().Length >= 2
            )
            .When(x => !string.IsNullOrWhiteSpace(x.Title))
            .WithMessage("Title must be at least 2 characters long.");

        RuleFor(x => x.Title)
            .Must(title =>
                string.IsNullOrWhiteSpace(title) ||
                title.Trim().Length <= 32
            )
            .When(x => !string.IsNullOrWhiteSpace(x.Title))
            .WithMessage("Title must be no longer than 32 characters.");
    }
}