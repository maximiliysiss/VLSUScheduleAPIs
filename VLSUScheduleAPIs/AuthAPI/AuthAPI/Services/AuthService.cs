using AuthAPI.Models;
using Commonlibrary.Models;
using Commonlibrary.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace AuthAPI.Services
{
    public interface IAuthService
    {
        TokenResult AuthAttempt(LoginModel loginModel);
        TokenResult RefreshToken(string token, string refreshToken);
    }

    public class AuthService : IAuthService
    {
        private readonly DatabaseContext databaseContext;
        private readonly AuthorizeSettings authorizeSettings;

        public AuthService(DatabaseContext databaseContext, AuthorizeSettings authorizeSettings)
        {
            this.databaseContext = databaseContext;
            this.authorizeSettings = authorizeSettings;
        }

        private string GenerateToken(User user, int days)
        {
            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                issuer: authorizeSettings.Issuer,
                audience: authorizeSettings.Audience,
                claims: user.ClaimsIdentity.Claims,
                notBefore: now,
                expires: now.AddDays(days),
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authorizeSettings.SecurityKey, SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public TokenResult AuthAttempt(LoginModel loginModel)
        {
            var user = databaseContext.Users.FirstOrDefault(x => x.Login == loginModel.Login && x.Password == loginModel.Password);
            if (user == null)
                return null;

            user.Token = GenerateToken(user, authorizeSettings.RefreshExpiration);
            databaseContext.Update(user);
            databaseContext.SaveChanges();

            return new TokenResult
            {
                JWTRefresh = user.Token,
                JWTAccess = GenerateToken(user, authorizeSettings.AccessExpiration),
                UserType = user.UserType
            };
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidAudience = authorizeSettings.Audience,
                ValidIssuer = authorizeSettings.Issuer,
                IssuerSigningKey = authorizeSettings.SecurityKey
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public TokenResult RefreshToken(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            var user = databaseContext.Users.FirstOrDefault(x => x.Login == principal.Identity.Name);
            if (user == null || user.Token != refreshToken)
                return null;
            var newJwt = GenerateToken(user, authorizeSettings.AccessExpiration);
            user.Token = GenerateToken(user, authorizeSettings.RefreshExpiration);

            databaseContext.Update(user);
            databaseContext.SaveChanges();

            return new TokenResult { JWTAccess = newJwt, JWTRefresh = user.Token, UserType = user.UserType };
        }
    }
}
