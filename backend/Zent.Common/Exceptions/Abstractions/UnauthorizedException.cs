namespace Zent.Common.Exceptions.Abstractions;

public abstract class UnauthorizedException(string message) : ZentException(message);