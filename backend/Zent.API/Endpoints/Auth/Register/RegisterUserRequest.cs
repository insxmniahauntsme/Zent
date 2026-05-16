namespace Zent.API.Endpoints.Auth.Register;

public sealed record RegisterUserRequest(string Email, string Password, string FirstName, string LastName);