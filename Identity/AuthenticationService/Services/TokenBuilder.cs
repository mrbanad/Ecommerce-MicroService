using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Services;

public class TokenBuilder : ITokenBuilder
{
    public string BuildToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey("sadawdwdawdadsdawdasdawdaxdwadaxcxcxcxcssa"u8.ToArray());

        var token = new JwtSecurityToken(
            issuer: "https://localhost",
            audience: "https://localhost:4200",
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}