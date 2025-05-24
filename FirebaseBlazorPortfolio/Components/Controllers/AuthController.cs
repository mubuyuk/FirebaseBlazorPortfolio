using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Firebase.Auth;
using Firebase.Auth.Providers;

namespace FirebaseBlazorPortfolio.Components.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly FirebaseAuthClient _firebaseAuthClient;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, ILogger<AuthController> logger)
        {
            _logger = logger;

            var authConfig = new FirebaseAuthConfig
            {
                ApiKey = config["Firebase:ApiKey"],
                AuthDomain = config["Firebase:AuthDomain"],
                Providers = [new EmailProvider()]
            };

            _firebaseAuthClient = new FirebaseAuthClient(authConfig);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest credentials)
        {
            try
            {
                var result = await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(
                    credentials.Email, credentials.Password);
                var token = await result.User.GetIdTokenAsync();

                _logger.LogInformation("Firebase login successful: UID={Uid}, Email={Email}",
                    result.User.Uid, result.User.Info.Email);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, result.User.Uid),
                    new(ClaimTypes.Email, result.User.Info.Email),
                    new("FirebaseToken", token)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    });

                return Redirect("/admin");
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogWarning(ex, "Login failed");
                return Unauthorized("Felaktig e-post eller lösenord.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return StatusCode(500, "Ett internt fel inträffade.");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Användaren har loggats ut.");
            return Ok();
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
