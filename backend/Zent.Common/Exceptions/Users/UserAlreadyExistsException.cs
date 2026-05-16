using System.Net;
using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class UserAlreadyExistsException(string message) : ConflictException(message)
{
    public override string ErrorCode => ErrorCodes.UserAlreadyExists;
}