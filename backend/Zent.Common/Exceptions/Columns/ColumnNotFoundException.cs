using Zent.Common.Exceptions.Abstractions;

namespace Zent.Common.Exceptions;

public sealed class ColumnNotFoundException(string message) : NotFoundException(message)
{
    public override string ErrorCode => ErrorCodes.ColumnNotFound;
}