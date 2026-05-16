using FluentValidation;

namespace Zent.API.Endpoints.Users.SearchUsers;

public sealed class SearchUsersRequestValidator : AbstractValidator<SearchUsersRequest>
{
    public SearchUsersRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty()
            .MinimumLength(3)
            .WithMessage("Search query must be at least 3 characters")
            .Must(x => !x.Trim().StartsWith('@'))
            .WithMessage("Search query cannot start with '@'.");
    }
}