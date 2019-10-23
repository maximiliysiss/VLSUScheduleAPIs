using AuthAPI.Models;
using AuthAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        public ActionResult<TokenResult> Login(LoginModel loginModel)
        {
            var loginResult = authService.AuthAttempt(loginModel);
            if (loginResult == null)
                return NotFound();
            return loginResult;
        }

        [HttpGet]
        public ActionResult<TokenResult> Refresh()
        {
            var token = Request.Headers["token"];
            var refreshToken = Request.Headers["refresh"];
            if (token.Count != 1 || refreshToken.Count != 1)
                return BadRequest();

            var refreshResult = authService.RefreshToken(token[0] ?? string.Empty, refreshToken[0] ?? string.Empty);
            if (refreshResult == null)
                return BadRequest();

            return refreshResult;
        }

        [Authorize]
        public ActionResult Try() => Ok();
    }
}