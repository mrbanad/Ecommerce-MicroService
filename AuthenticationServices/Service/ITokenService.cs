using System.Security.Claims;

namespace AuthenticationServices.Service;

public interface ITokenService
{
    string BuildToken(IEnumerable<Claim> authClaims);
}