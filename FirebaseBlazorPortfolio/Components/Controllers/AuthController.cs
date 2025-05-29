using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using FirebaseBlazorPortfolio.Services;
using Firebase.Auth;

namespace FirebaseBlazorPortfolio.Components.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly FirebaseAuthService _firebaseAuthService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            FirebaseAuthService firebaseAuthService,
            ILogger<AuthController> logger)
        {
            _firebaseAuthService = firebaseAuthService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest credentials)
        {
            try
            {
                var success = await _firebaseAuthService.SignInAsync(credentials.Email, credentials.Password);

                if (!success)
                {
                    return Unauthorized("Felaktig e-post eller lösenord.");
                }

                _logger.LogInformation("Inloggning lyckades för {Email}", credentials.Email);
                return Redirect("/admin");
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogWarning(ex, "Firebase-autentisering misslyckades");
                return Unauthorized("Felaktig e-post eller lösenord.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett oväntat fel uppstod vid inloggning");
                return StatusCode(500, "Ett internt fel inträffade.");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _firebaseAuthService.LogoutAsync();
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
