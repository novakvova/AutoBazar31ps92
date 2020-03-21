using AutoBazar.BLL.Abstract;
using AutoBazar.BLL.DTO;
using AutoBazar.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBazar.BLL.Services
{
    public class JWTTokenService : IJWTTokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<DbUser> _userManager;
        private readonly CancellationToken _cancellationToken;
        public JWTTokenService(ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<DbUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
        }
        public string CreateRefreshToken(DbUser user)
        {
            return "";
        }
        public string CreateToken(DbUser user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;
            var claims = new List<Claim>()
            {
                //new Claim(JwtRegisteredClaimNames.Sub, user.Id)
                new Claim("id", user.Id.ToString()),
                new Claim("name", user.UserName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }

            string jwtTokenSecretKey = this._configuration.GetValue<string>("SecretPhrase");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSecretKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                expires: DateTime.Now.AddYears(1));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        public async Task<TokensDTO> RefreshAuthToken(string oldAuthToken, string refreshToken)
        {
            string apiKey = _configuration.GetValue<string>("ApiKey");
            var principal = AuthHelpers.GetPrincipalFromExpiredToken(apiKey, oldAuthToken);
            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userId = long.Parse(userIdStr, new NumberFormatInfo());
            var userRefreshToken = await _context.UserRefreshTokens
                .SingleOrDefaultAsync(x => x.UserId == userId && x.RefreshToken == refreshToken, _cancellationToken)
                .ConfigureAwait(false);
            if (userRefreshToken == null)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }
            _context.UserRefreshTokens.Remove(userRefreshToken);
            if (userRefreshToken.ExpiryDate < DateTime.UtcNow)
            {

                await _context.SaveChangesAsync(_cancellationToken).ConfigureAwait(false);
                throw new SecurityTokenException("Token is expired");
            }
            var newJwtToken = AuthHelpers.GenerateToken(apiKey, principal.Claims);
            var newRefreshToken = AuthHelpers.GenerateRefreshToken();
            await _context.UserRefreshTokens.AddAsync(new UserRefreshToken
            {
                UserId = userId,
                RefreshToken = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddMonths(1),
            }, _cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(_cancellationToken).ConfigureAwait(false);
            return new TokensDTO(newJwtToken, newRefreshToken);
        }

    }
}
