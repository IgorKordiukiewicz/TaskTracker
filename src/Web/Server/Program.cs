using Web.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Application;
using Serilog;
using System.Security.Claims;
using Web.Server.Requirements;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.WriteTo.File("log.txt", rollingInterval: RollingInterval.Month, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning);
});

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
    {
        c.Authority = builder.Configuration["Auth0:Domain"];
        c.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidAudience = builder.Configuration["Auth0:Audience"],
            ValidIssuer = builder.Configuration["Auth0:Domain"]
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OrganizationMember", policy => policy.Requirements.Add(new OrganizationMemberRequirement())); // TODO: Check if policy.RequireAuthenticatedUser() should be added?
});
builder.Services.AddScoped<IAuthorizationHandler, OrganizationMemberHandler>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllers();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { } // Required for integration testing