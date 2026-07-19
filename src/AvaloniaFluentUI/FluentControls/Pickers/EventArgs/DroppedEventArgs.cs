using System;
using System.Collections.Generic;

namespace AvaloniaFluentUI.Controls;

public class DroppedEventArgs(IReadOnlyList<string> values) : EventArgs
{
    public IReadOnlyList<string> Values { get; } = values;
}
