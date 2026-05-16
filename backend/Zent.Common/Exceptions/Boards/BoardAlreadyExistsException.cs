using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class BoardAlreadyExistsException(string message) : ConflictException(message)
{
    public override string ErrorCode => ErrorCodes.BoardAlreadyExists;
}