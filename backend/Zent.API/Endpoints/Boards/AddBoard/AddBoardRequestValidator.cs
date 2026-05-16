using FluentValidation;

namespace Zent.API.Endpoints.Boards.AddBoard;

public sealed class AddBoardRequestValidator : AbstractValidator<AddBoardRequest>
{
    public AddBoardRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 40);

        RuleFor(x => x.Description)
            .MaximumLength(128);
    }
}