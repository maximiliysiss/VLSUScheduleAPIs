using Microsoft.AspNetCore.Mvc;
using AuthAPI.Models;
using AuthAPI.Services;
using Commonlibrary.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;
using System.Text;
using Commonlibrary.Controllers;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        DatabaseContext databaseContext;
        TokenManagement tokenManagement;

        public AuthController(DatabaseContext database, TokenManagement tokenManagement)
        {
            this.databaseContext = database;
            this.tokenManagement = tokenManagement;
        }

        public async Task Token(string username)
        {
            var identity = GetIdentity(username);
            if (identity == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Invalid username or password.");
                return;
            }

            var jwt = new JwtSecurityToken(
                    issuer: tokenManagement.Issuer,
                    audience: tokenManagement.Audience,
                    notBefore: DateTime.Now,
                    claims: identity.Claims,
                    expires: DateTime.Now.Add(TimeSpan.FromMinutes(tokenManagement.RefreshExpiration)),
                    signingCredentials: new SigningCredentials(tokenManagement.SymmetricKey, SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private ClaimsIdentity GetIdentity(string username)
        {
            User person = databaseContext.Users.FirstOrDefault(x => x.Login == username);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.UserType.ToString())
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }

        [Authorize]
        public string Test()
        {
            return "Hello world!";
        }
    }
}