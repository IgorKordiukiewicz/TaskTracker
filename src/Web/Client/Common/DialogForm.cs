﻿namespace Web.Client.Common;

public abstract class DialogForm
{
    public bool Visible { get; set; }

    public void Open() => Visible = true;
    public void Close() => Visible = false;

    public abstract bool IsValid();
}
