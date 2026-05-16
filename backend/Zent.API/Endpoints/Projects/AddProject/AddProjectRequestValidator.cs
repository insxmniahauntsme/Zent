using FluentValidation;

namespace Zent.API.Endpoints.Projects.AddProject;

public sealed class AddProjectRequestValidator : AbstractValidator<AddProjectRequest>
{
    public AddProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Must(x => x.Trim().Length >= 2)
            .WithMessage("Project name must be between 2 and 20 characters.");
        
        RuleFor(x => x.Description)
            .MaximumLength(128)
            .WithMessage("Project description must be less than 128 characters.");
        
        RuleFor(x => x.Client)
            .MaximumLength(128)
            .WithMessage("Client name must be less than 32 characters.");
    }
}