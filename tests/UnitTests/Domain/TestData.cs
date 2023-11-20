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

public class TestRole : Role<TestPermissions>
{
    public TestRole(string name, TestPermissions permissions, RoleType type = RoleType.Custom)
        : base(name, permissions, type)
    {
    }
}

public class TestMember : IHasRole
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid RoleId { get; set; }

    public TestMember(Guid roleId)
    {
        RoleId = roleId;
    }

    public void UpdateRole(Guid roleId) { }
}
