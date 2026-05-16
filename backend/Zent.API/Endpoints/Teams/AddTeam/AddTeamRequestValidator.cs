using FluentValidation;

namespace Zent.API.Endpoints.Teams.AddTeam;

public sealed class AddTeamRequestValidator : AbstractValidator<AddTeamRequest>
{
    public AddTeamRequestValidator()
    {
        RuleFor(x => x.Name)
            .Length(2, 20)
            .WithMessage("Team name must be between 2 and 20 characters.");
    }
}