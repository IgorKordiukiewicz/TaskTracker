using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Web.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Web.Client.Components;
using Blazored.LocalStorage;
using Web.Client.RequirementHandlers;
using Microsoft.AspNetCore.Authorization;
using Shared.Authorization;
using Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("ServerAPI",
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// TODO: Extract to a separate class
builder.Services.AddScoped<UserDataService>();
builder.Services.AddScoped<HierarchyNavigationService>();
builder.Services.AddScoped<OrganizationsService>();
builder.Services.AddScoped<WorkflowsService>();
builder.Services.AddScoped<ProjectsService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<TasksService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

builder.Services.AddAuthorizationCore(options => options.AddPolicies());
builder.Services.AddScoped<IAuthorizationHandler, OrganizationMemberRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ProjectMemberRequirementHandler>();
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
});

builder.Services.AddMudServices();

await builder.Build().RunAsync();
