namespace Zent.Common.Exceptions.Abstractions;

public abstract class ForbiddenException(string message) : ZentException(message);