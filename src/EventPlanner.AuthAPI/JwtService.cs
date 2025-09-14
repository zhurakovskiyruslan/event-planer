using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventPlanner.AuthAPI.Data;
using EventPlanner.AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EventPlanner.AuthAPI;

public class JwtService
{
    private readonly IConfiguration _config;
    private readonly UserManager<ApplicationUser> _users;
    private readonly AppIdentityDbContext _db;

    public JwtService(IConfiguration config, UserManager<ApplicationUser> users,  AppIdentityDbContext db)
    {
        _config = config;
        _users = users;
        _db = db;
    }
    public string Issue(ApplicationUser user, int domainUserId, IEnumerable<string>? roles = null)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Identity Id
            new Claim("userId", domainUserId.ToString()),               // Domain Id
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

        if (roles != null)
        {
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));
        }

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:  _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims:  claims,
            expires: DateTime.UtcNow.AddHours(int.Parse(_config["Jwt:LifetimeHours"] ?? "60")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}