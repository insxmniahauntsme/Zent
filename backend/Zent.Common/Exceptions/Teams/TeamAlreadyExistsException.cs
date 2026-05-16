using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public class TeamAlreadyExistsException(string message) : ConflictException(message)
{
    public override string? ErrorCode => ErrorCodes.TeamAlreadyExists;
}