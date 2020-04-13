namespace Bejebeje.Admin
{
  using System.IdentityModel.Tokens.Jwt;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.HttpOverrides;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Services;
  using Services.Config;

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      string authority = Configuration["IdentityServerConfiguration:Authority"];
      string clientId = Configuration["IdentityServerConfiguration:ClientId"];
      string clientSecret = Configuration["IdentityServerConfiguration:ClientSecret"];

      services.AddScoped<IArtistService, ArtistService>();

      services.AddScoped<IArtistSlugService, ArtistSlugService>();

      services.AddScoped<ILyricService, LyricService>();

      services.AddScoped<ILyricSlugService, LyricSlugService>();

      services.AddScoped<IArtistImageService, ArtistImageService>();

      services.Configure<DatabaseOptions>(Configuration);

      JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

      services.AddAuthentication(options =>
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
        });

      services
        .AddControllersWithViews()
        .AddRazorRuntimeCompilation();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");

        app.UseHsts();
      }

      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      });

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
    }
  }
}
