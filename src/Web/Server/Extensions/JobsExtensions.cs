using Application.Common;

namespace Web.Server.Extensions;

public static class JobsExtensions
{
    public static void AddCRONJobs(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var jobsService = scope.ServiceProvider.GetRequiredService<IJobsService>();

        jobsService.AddExpireOrganizationsInvitationsJob();
    }
}
