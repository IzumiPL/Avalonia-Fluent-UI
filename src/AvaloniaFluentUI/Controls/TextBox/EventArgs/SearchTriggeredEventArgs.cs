using System;

namespace AvaloniaFluentUI.Controls;

public class SearchTriggeredEventArgs : EventArgs
{
    public string? Value { get; }

    public SearchTriggeredEventArgs(string? value)
    {
        Value = value;
    }
}
