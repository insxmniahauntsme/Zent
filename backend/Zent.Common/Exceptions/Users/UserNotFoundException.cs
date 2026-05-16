using System.Net;
using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class UserNotFoundException(string message) : NotFoundException(message)
{
    public override string ErrorCode => ErrorCodes.UserNotFound;
}