using Application;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure;
using Serilog;
using System.Reflection;
using Web.Server.Configuration;
using Microsoft.Extensions.Logging.ApplicationInsights;
using HangfireBasicAuthenticationFilter;
using Analytics;

var builder = WebApplication.CreateBuilder(args);

var configurationSettingsSection = builder.Configuration.GetSection("ConfigurationSettings");
builder.Services.Configure<ConfigurationSettings>(configurationSettingsSection);
var configurationSettings = configurationSettingsSection.Get<ConfigurationSettings>()
    ?? throw new InvalidOperationException("ConfigurationSettings is not correct.");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.IncludeXmlComments(
    Path.Combine(AppContext.BaseDirectory,$"{Assembly.GetExecutingAssembly().GetName().Name}.xml")));

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(x => x.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddHangfireServer();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAnalyticsServices(builder.Configuration);
builder.Services.AddAuth(builder.Configuration);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();

builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) =>
        config.ConnectionString = builder.Configuration.GetConnectionString("AppInsightsConnection"),
        configureApplicationInsightsLoggerOptions: (options) => { }
    );
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(null, LogLevel.Trace);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Web.Client", builder =>
    {
        builder
        .SetIsOriginAllowed(origin => origin.Contains(configurationSettings.Domain))
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors("Web.Client");

if(app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}
else
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[]
        {
            new HangfireCustomBasicAuthenticationFilter()
            {
                User = "admin",
                Pass = builder.Configuration.GetSection("Authentication:HangfirePassword").Value
            }
        }
    });
}

app.AddCRONJobs();

app.Run();

public partial class Program { } // Required for integration testing