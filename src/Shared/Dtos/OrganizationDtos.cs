﻿namespace Shared.Dtos;

public record CreateOrganizationDto(string Name, Guid OwnerId);

public record CreateOrganizationInvitationDto(Guid UserId);

public record RemoveOrganizationMemberDto(Guid MemberId);
