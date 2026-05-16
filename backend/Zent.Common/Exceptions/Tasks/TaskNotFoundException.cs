using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class TaskNotFoundException(string message) : NotFoundException(message)
{
    public override string ErrorCode => ErrorCodes.TaskNotFound;
}