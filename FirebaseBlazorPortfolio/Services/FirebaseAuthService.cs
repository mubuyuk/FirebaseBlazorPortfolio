using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace FirebaseBlazorPortfolio.Services;

public class FirebaseAuthService
{
    private readonly FirebaseAuthClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FirebaseAuthService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        var firebaseConfig = new FirebaseAuthConfig
        {
            ApiKey = config["Firebase:ApiKey"],
            AuthDomain = config["Firebase:AuthDomain"],
            Providers = [new EmailProvider()]
        };

        _client = new FirebaseAuthClient(firebaseConfig);
    }

    public async Task<bool> SignInAsync(string email, string password)
    {
        var result = await _client.SignInWithEmailAndPasswordAsync(email, password);
        var token = await result.User.GetIdTokenAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.User.Uid),
            new(ClaimTypes.Email, result.User.Info.Email),
            new("FirebaseToken", token)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

        return true;
    }

    public async Task LogoutAsync()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
