using System.Net;

namespace Zent.Common.Exceptions.Abstractions;

public abstract class ZentException(string message) : Exception(message)
{
    public virtual string? ErrorCode => null;
}