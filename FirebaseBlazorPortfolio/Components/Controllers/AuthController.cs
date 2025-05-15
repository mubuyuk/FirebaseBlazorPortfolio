using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private static bool firebaseInitialized = false;

    public AuthController(IWebHostEnvironment env)
    {
        if (!firebaseInitialized)
        {
            var path = Path.Combine(env.ContentRootPath, "Keys", "firebase-adminsdk.json");
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(path)
            });
            firebaseInitialized = true;
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] TokenDto dto)
    {
        var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(dto.IdToken);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
            new Claim(ClaimTypes.Email, decodedToken.Claims["email"].ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return Ok();
    }

    public class TokenDto
    {
        public string IdToken { get; set; } = string.Empty;
    }
}
