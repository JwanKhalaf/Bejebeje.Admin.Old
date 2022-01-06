using Bejebeje.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Bejebeje.Services.Config;

var builder = WebApplication.CreateBuilder(args);

// add services to the container.
IdentityModelEventSource.ShowPII = true;
string authority = builder.Configuration["IdentityServerConfiguration:Authority"];
string clientId = builder.Configuration["IdentityServerConfiguration:ClientId"];
string clientSecret = builder.Configuration["IdentityServerConfiguration:ClientSecret"];

builder.Services.Configure<DatabaseOptions>(builder.Configuration);

builder.Services.AddScoped<IArtistService, ArtistService>();

builder.Services.AddScoped<IArtistSlugService, ArtistSlugService>();

builder.Services.AddScoped<ILyricService, LyricService>();

builder.Services.AddScoped<ILyricSlugService, LyricSlugService>();

builder.Services.AddScoped<IArtistImageService, ArtistImageService>();

builder.Services.AddAuthentication(options =>
    {
      options.DefaultScheme = "Cookies";
      options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
      options.Authority = authority;
      options.RequireHttpsMetadata = false;
      options.ClientId = clientId;
      options.ClientSecret = clientSecret;
      options.ResponseType = "code";
      options.SaveTokens = true;
      options.GetClaimsFromUserInfoEndpoint = true;
      options.Scope.Clear();
      options.Scope.Add("openid");
      options.ClaimActions.MapUniqueJsonKey("role", "role");
      options.TokenValidationParameters = new TokenValidationParameters { NameClaimType = "cognito:user", RoleClaimType = "cognito:groups" };
    });

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var app = builder.Build();

ForwardedHeadersOptions forwardedHeadersOptions = new ForwardedHeadersOptions
{
  ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};

forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedHeadersOptions);

// configure the http request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // the default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
      {
        endpoints
          .MapControllerRoute(name: "default", pattern: "{controller=Artist}/{action=Index}/{id?}")
          .RequireAuthorization();
      });

app.Run();
