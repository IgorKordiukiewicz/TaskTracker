using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Web.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Web.Client.Components;
using Web.Client.Common;
using Blazored.LocalStorage;
using Web.Client.Requirements;
using Microsoft.AspNetCore.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("ServerAPI",
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped<RequestHandler>();
builder.Services.AddScoped<UserDataService>();
builder.Services.AddScoped<HierarchyNavigationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("OrganizationMember", policy => policy.Requirements.Add(new OrganizationMemberRequirement()));
    options.AddPolicy("ProjectMember", policy => policy.Requirements.Add(new ProjectMemberRequirement()));
});
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
