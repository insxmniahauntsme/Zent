namespace Zent.API;

public class ErrorResponse
{
    public string? Code { get; set; }
    public required string Message { get; set; }
}