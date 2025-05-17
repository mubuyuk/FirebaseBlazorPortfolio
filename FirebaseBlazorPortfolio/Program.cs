using FirebaseBlazorPortfolio.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using MyPortfolio.Services;

namespace FirebaseBlazorPortfolio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Razor Components / Blazor Server
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // 2. Firestore & tjänster
            builder.Services.AddSingleton<FirestoreService>();
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            // 3. Cookiebaserad autentisering
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/login";
                options.Cookie.SameSite = SameSiteMode.Lax; // Viktigt!
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });


            builder.Services.AddAuthorization();
            builder.Services.AddAuthorizationCore();

            // 4. Blazor AuthenticationStateProvider (utan Identity)
            builder.Services.AddScoped<ServerAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
                provider.GetRequiredService<ServerAuthenticationStateProvider>());

            // 5. HTTP-klient
            builder.Services.AddHttpClient("Default", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7090"); // Justera vid behov
            });
            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("Default"));

            // 6. Bygg och konfigurera appen
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(); // viktigt före auth

            app.UseAuthentication(); // måste komma före Authorization
            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
