using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Auth.Register;

public sealed record RegisterUserCommand(string Email, string Password, string FirstName, string LastName)
    : ICommand<string>;