namespace Zent.Common.Exceptions.Abstractions;

public abstract class NotFoundException(string message) : ZentException(message);