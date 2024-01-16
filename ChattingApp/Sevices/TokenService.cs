using ChattingApp.Interfaces;
using ChattingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChattingApp.Sevices
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> userManager;

        public TokenService(IConfiguration config,UserManager<AppUser> userManager)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            this.userManager = userManager;
        }
        public async  Task<string> CreateToken(AppUser user)
        {
            var claims = new List<Claim>
           {
               new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
               new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
           };

            var roles = await userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role=> new Claim(ClaimTypes.Role, role)));

            //SecurityTokenDescriptor is a class in the Microsoft.IdentityModel.Tokens namespace that represents the metadata needed to create a security token, such as a JSON Web Token (JWT).
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokendescriptor = new SecurityTokenDescriptor

            {

                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokendescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
