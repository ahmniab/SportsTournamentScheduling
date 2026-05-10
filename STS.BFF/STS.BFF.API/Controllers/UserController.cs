using Microsoft.AspNetCore.Mvc;

namespace STS.BFF.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }

        
        return Ok(new
        {
            IsAuthenticated = true,
            Username = User.Identity.Name,
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
            FirstName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value,
            LastName = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value,
            Roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value)
        });
    }
}