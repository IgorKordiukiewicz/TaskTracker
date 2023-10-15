﻿namespace Shared.ViewModels;

public record ProjectMembersVM(IReadOnlyList<ProjectMemberVM> Members);
public record ProjectMemberVM(Guid Id, string Name);