using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class TeamAccessDeniedException(string message) : ForbiddenException(message)
{
    public override string ErrorCode => ErrorCodes.TeamAccessDenied;
}