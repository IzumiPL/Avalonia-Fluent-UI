using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Gallery.Services;

public class JumpModel
{
    public string Page { get; set; }
    public string ControlName { get; set; }
}

public class JumpService
{
    public static event EventHandler<JumpModel>? OnJumpToControl;

    public static void InvokeJumpEvent(JumpModel model)
    {
        OnJumpToControl?.Invoke(null, model);
    }

    public static void GotoControl(Button button)
    {
        var name = button.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault(x => x.Name == "Title")?.Text;
        var page = button.Tag?.ToString()!;

        InvokeJumpEvent(new JumpModel
        {
            Page = page,
            ControlName = name
        });

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"JumpService.GotoFromButton => Page: {page}, Name: {name}");
#endif
    }
}
