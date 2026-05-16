namespace Zent.Common;

public static class ValidationPatterns
{
    public const string Email = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
    public const string Password = @"^(?=.*[A-Za-z]).{8,}$";
}