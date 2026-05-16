using FluentValidation;

namespace Zent.API.Endpoints.Columns.MoveColumn;

public sealed class MoveColumnRequestValidator : AbstractValidator<MoveColumnRequest>
{
    public MoveColumnRequestValidator()
    {
        RuleFor(x => x.TargetOrder)
            .GreaterThanOrEqualTo(1);
    }
}