using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationServices.Service;

public class TokenService : ITokenService
{
    public string BuildToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey("sadawdwdawdadsdawdasdawdaxdwadaxcxcxcxcssa"u8.ToArray());

        var token = new JwtSecurityToken(
            issuer: "https://localhost",
            audience: "https://localhost",
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}