using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class ProjectAlreadyExistsException(string message) : ConflictException(message)
{
    public override string ErrorCode => ErrorCodes.ProjectAlreadyExists;
}