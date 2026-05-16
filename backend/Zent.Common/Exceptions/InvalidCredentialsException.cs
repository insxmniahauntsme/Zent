using System.Net;
using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class InvalidCredentialsException(string message) : UnauthorizedException(message)
{
    public override string ErrorCode => ErrorCodes.InvalidCredentials;
}