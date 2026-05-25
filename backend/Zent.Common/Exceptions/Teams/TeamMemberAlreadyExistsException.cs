using System.Runtime.InteropServices.JavaScript;
using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class TeamMemberAlreadyExistsException(string message) : ConflictException(message)
{
    public override string ErrorCode => ErrorCodes.TeamMemberAlreadyExists;
}