using Domain.Models;

namespace Application.Interface
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(User user);
    }
}