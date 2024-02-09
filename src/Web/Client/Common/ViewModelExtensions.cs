using Shared.ViewModels;

namespace Web.Client.Common;

// TODO: Move to services?
public static class ViewModelExtensions
{
    public static IReadOnlyList<string> GetAvailableFromStatuses(this WorkflowVM? workflowVM)
    {
        if(workflowVM is null)
        {
            return Array.Empty<string>();
        }

        var result = new List<string>();
        foreach (var status in workflowVM.Statuses)
        {
            var fromTransitionsCount = workflowVM.Transitions.Count(x => x.FromStatusId == status.Id);
            if (fromTransitionsCount < workflowVM.Statuses.Count - 1)
            {
                result.Add(status.Name);
            }
        }

        return result;
    }

    public static IReadOnlyList<string> GetAvailableToStatuses(this WorkflowVM? workflowVM, string fromStatus, IReadOnlyDictionary<string, Guid>? statusIdByName)
    {
        if (workflowVM is null || statusIdByName is null)
        {
            return Array.Empty<string>();
        }

        var fromStatusId = statusIdByName[fromStatus];
        var availableStatusesIds = workflowVM.Transitions.Where(x => x.FromStatusId == fromStatusId)
            .Select(x => x.ToStatusId).ToHashSet();

        return workflowVM
            .Statuses.Where(x => !availableStatusesIds.Contains(x.Id) && x.Id != fromStatusId)
            .Select(x => x.Name)
            .ToList();
    }

    public static ProjectMemberVM? GetCurrentAssigneeVM(this ProjectMembersVM membersVM, Guid? assigneeId)
    {
        return membersVM.Members.FirstOrDefault(x => x.UserId == assigneeId) ?? null;
    }

    public static IEnumerable<ProjectMemberVM> GetPossibleAssignees(this ProjectMembersVM membersVM, Guid? assigneeId)
    {
        if (assigneeId is not null && assigneeId != Guid.Empty)
        {
            return membersVM.Members.Where(x => x.UserId != assigneeId.Value).Append(new(Guid.Empty, Guid.Empty, "-", Guid.Empty)).Reverse();
        }
        else
        {
            return membersVM.Members;
        }
    }
}
