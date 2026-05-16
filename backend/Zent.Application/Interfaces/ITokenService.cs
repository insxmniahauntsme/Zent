using Zent.Data.Entities;

namespace Zent.Application.Interfaces;

public interface ITokenService
{
    public string GenerateToken(UserEntity user);
}