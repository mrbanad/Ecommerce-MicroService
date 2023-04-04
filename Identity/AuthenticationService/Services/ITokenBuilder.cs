using System.Security.Claims;

namespace AuthenticationService.Services;

public interface ITokenBuilder
{
        string BuildToken(IEnumerable<Claim> authClaims);
}