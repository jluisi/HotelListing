using System;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using HotelListing.Data;
using HotelListing.DTOS.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace HotelListing.Services
{
  public class AuthManager : IAuthManager
  {
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;
    private ApiUser _user;

    //---------------------------------------------------------------------------------------------
    public AuthManager(UserManager<ApiUser> userManager, IConfiguration configuration)
    {
      _userManager = userManager;
      _configuration = configuration;
    }

    //---------------------------------------------------------------------------------------------
    // Check if the User exists in the system and the password is correct
    //---------------------------------------------------------------------------------------------
    public async Task<bool> ValidateUser(LoginDTO loginDTO)
    {
      _user = await _userManager.FindByNameAsync(loginDTO.Email);
      var validPassword = await _userManager.CheckPasswordAsync(_user, loginDTO.Password);
      
      return (_user != null && validPassword);
    }

    //---------------------------------------------------------------------------------------------
    // Creates a Token for the signing credentials and claims
    //---------------------------------------------------------------------------------------------
    public async Task<string> CreateToken()
    {
      var signingCredentials = GetSigningCredentials();
      var claims = await GetClaims();
      var token = GenerateTokenOptions(signingCredentials, claims);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    //---------------------------------------------------------------------------------------------
    private SigningCredentials GetSigningCredentials()
    {
      var key = Environment.GetEnvironmentVariable("KEY");
      var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

      return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    //---------------------------------------------------------------------------------------------
    private async Task<List<Claim>> GetClaims()
    {
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Name, _user.UserName)
      };

      var roles = await _userManager.GetRolesAsync(_user);

      foreach (var role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }

      return claims;
    }

    //---------------------------------------------------------------------------------------------
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials,
      List<Claim> claims)
    {
      var jwtSettings = _configuration.GetSection("Jwt");

      var minutesToExpire = Convert.ToDouble(jwtSettings.GetSection("Lifetime").Value);
      var expiration = DateTime.Now.AddMinutes(minutesToExpire);

      var token = new JwtSecurityToken(
        issuer: jwtSettings.GetSection("Issuer").Value,
        claims: claims,
        expires: expiration,
        signingCredentials: signingCredentials
      );

      return token;
    }

  }
}
