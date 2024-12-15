using Domain.Common;

namespace UnitTests.Domain;

[Flags]
public enum TestPermissions
{
    None = 0,
    A = 1 << 0, // 1
    B = 1 << 1, // 2
    C = 1 << 2, // 4
}

public class TestRole(string name, TestPermissions permissions, RoleType type = RoleType.Custom) 
    : Role<TestPermissions>(name, permissions, type)
{
}

public class TestMember(Guid roleId)
    : IHasRole
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = Guid.NewGuid();

    public Guid RoleId { get; set; } = roleId;

    public void UpdateRole(Guid roleId) { }
}
