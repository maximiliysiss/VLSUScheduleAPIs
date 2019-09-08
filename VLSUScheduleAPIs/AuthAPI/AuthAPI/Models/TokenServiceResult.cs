using Commonlibrary.Controllers;

namespace AuthAPI.Models
{
    public class TokenServiceResult : PrimitiveServiceResult
    {
        public string Token { get; set; }
    }
}