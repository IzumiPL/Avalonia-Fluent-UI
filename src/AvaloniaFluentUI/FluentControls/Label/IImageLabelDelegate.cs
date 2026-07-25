using Avalonia;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

public interface IImageLabelDelegate
{
    void Render(DrawingContext context, Rect rect, CornerRadius radius);
}
