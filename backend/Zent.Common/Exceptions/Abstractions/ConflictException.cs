namespace Zent.Common.Exceptions.Abstractions;

public abstract class ConflictException(string message) : ZentException(message);