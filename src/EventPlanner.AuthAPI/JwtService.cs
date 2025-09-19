using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventPlanner.AuthAPI.Contracts;
using EventPlanner.AuthAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace EventPlanner.AuthAPI;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public JwtResult Issue(ApplicationUser user, IEnumerable<string>? roles = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
            new(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

        if (roles != null)
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var lifetimeHours = int.Parse(_config["Jwt:LifetimeHours"] ?? "1");
        var expiresAt = DateTime.UtcNow.AddHours(lifetimeHours);

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new JwtResult(tokenString, expiresAt);
    }
}