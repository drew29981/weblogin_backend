using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using wblg.contexts;
using wblg.controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    // options.LoginPath = "/api/login";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                    options.SlidingExpiration = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always; only for https (outside development) instead use app.Environment.IsDevelopment()
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });

builder.Services.AddCors(options =>
{
    options.AddPolicy("testmode", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
              
        policy.WithOrigins("https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddDbContext<UserContext>(options => { options.UseInMemoryDatabase("UserInMemoryDb"); });

builder.Services.AddControllers();

// builder.Services.AddHttpsRedirection(options => options.HttpsPort = 443);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("testmode");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
