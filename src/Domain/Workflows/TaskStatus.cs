﻿namespace Domain.Workflows;

public class TaskStatus : Entity
{
    public string Name { get; private set; }

    public bool Initial { get; private set; } = false;

    public int DisplayOrder { get; private set; }

    public TaskStatus(Guid id, string name, int displayOrder, bool initial = false)
        : base(id)
    {
        Name = name;
        Initial = initial;
        DisplayOrder = displayOrder;
    }
}