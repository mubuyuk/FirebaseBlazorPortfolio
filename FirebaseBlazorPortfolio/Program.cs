using FirebaseBlazorPortfolio.Components;
using FirebaseBlazorPortfolio.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace FirebaseBlazorPortfolio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAntiforgery();
            builder.Services.AddControllers();
            builder.Services.AddSingleton<FirestoreService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "FirebaseAuth.Cookie";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.LoginPath = "/login";
                });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

            var firebaseConfig = builder.Configuration.GetSection("Firebase");
            var apiKey = firebaseConfig["ApiKey"];
            var authDomain = firebaseConfig["AuthDomain"];

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Firebase API key is not configured");
            if (string.IsNullOrEmpty(authDomain))
                throw new Exception("Firebase Auth Domain is not configured");

            builder.Services.AddScoped(provider => new FirebaseAuthService(
                provider.GetRequiredService<IConfiguration>(),
                provider.GetRequiredService<IHttpContextAccessor>()));

            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:7090") });


            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
