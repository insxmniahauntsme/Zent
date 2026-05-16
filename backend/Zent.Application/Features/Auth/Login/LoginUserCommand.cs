using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Auth.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<string>;