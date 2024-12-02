using Application.Common;
using Hangfire;

namespace Web.Server.Extensions;

public static class JobsExtensions
{
    public static void UseJobs(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        var jobsService = scope.ServiceProvider.GetRequiredService<IJobsService>();

        recurringJobManager.AddOrUpdate(
            "Expire organizations invitations",
            () => jobsService.ExpireOrganizationsInvitations(default),
            "0 * * * *");
    }
}
