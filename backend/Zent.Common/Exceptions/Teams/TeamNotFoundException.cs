using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public class TeamNotFoundException(string message) : NotFoundException(message)
{
    public override string ErrorCode => ErrorCodes.TeamNotFound;
}