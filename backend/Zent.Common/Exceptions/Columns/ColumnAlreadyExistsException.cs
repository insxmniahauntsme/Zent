using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class ColumnAlreadyExistsException(string message) : ConflictException(message)
{
    public override string ErrorCode => ErrorCodes.ColumnAlreadyExists;
}