using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class ProjectNotFoundException(string message) : NotFoundException(message)
{
    public override string ErrorCode => ErrorCodes.ProjectNotFound;
}