using FluentValidation;
using Zent.Common.Enums;

namespace Zent.API.Endpoints.Teams.SearchTeamMembers;

public sealed class SearchTeamMembersRequestValidator : AbstractValidator<SearchTeamMembersRequest>
{
    public SearchTeamMembersRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty()
            .MinimumLength(3)
            .WithMessage("Search query must be at least 3 characters")
            .Must(x => !x.Trim().StartsWith('@'))
            .WithMessage("Search query cannot start with '@'.");
        
        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid role.");
        
        RuleFor(x => x.Specialization)
            .IsInEnum()
            .WithMessage("Invalid specialization.");
    }
}