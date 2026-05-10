using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace STS.BFF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase 
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = "http://localhost:3000")
        {
            return Challenge(new AuthenticationProperties 
            { 
                RedirectUri = returnUrl 
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}