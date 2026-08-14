using MarketSphere.Application.Common.Models;

namespace MarketSphere.Application.Common.Interfaces;

public interface IJwtTokenService
{
    TokenResult CreateToken(CurrentUserInfo user);
    string HashToken(string token);
}
