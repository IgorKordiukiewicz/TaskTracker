﻿namespace Domain.Projects;

public class ProjectMember : Entity
{
    public Guid UserId { get; private init; }
    public Guid RoleId { get; private set; }

    private ProjectMember(Guid id) 
        : base(id)
    {
    }

    public static ProjectMember Create(Guid userId, Guid roleId)
    {
        return new(Guid.NewGuid())
        {
            UserId = userId,
            RoleId = roleId
        };
    }

    public void UpdateRole(Guid roleId)
    {
        RoleId = roleId;
    }
}
