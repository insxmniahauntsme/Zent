using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class ProjectAccessDeniedException(string message) : ForbiddenException(message)
{
    public override string ErrorCode => ErrorCodes.ProjectAccessDenied;
}