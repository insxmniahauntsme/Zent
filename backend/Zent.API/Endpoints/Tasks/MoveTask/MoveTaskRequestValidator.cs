using FluentValidation;

namespace Zent.API.Endpoints.Tasks.MoveTask;

public sealed class MoveTaskRequestValidator : AbstractValidator<MoveTaskRequest>
{
    public MoveTaskRequestValidator()
    {
        RuleFor(x => x.TargetColumnId)
            .NotEmpty();

        RuleFor(x => x.TargetOrder)
            .GreaterThanOrEqualTo(1);
    }
}