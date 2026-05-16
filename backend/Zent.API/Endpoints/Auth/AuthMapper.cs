using Zent.API.Endpoints.Auth.Login;
using Zent.API.Endpoints.Auth.Register;
using Zent.Application.Features.Auth.Login;
using Zent.Application.Features.Auth.Register;

namespace Zent.API.Endpoints.Auth;

internal static class AuthMapper
{
    internal static RegisterUserCommand ToQuery(this RegisterUserRequest request) 
        => new(request.Email, request.Password, request.FirstName, request.LastName);
    
    internal static LoginUserCommand ToQuery(this LoginUserRequest request)
        => new(request.Email, request.Password);
}