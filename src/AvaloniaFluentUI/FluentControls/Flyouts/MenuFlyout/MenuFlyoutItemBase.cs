using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AvaloniaFluentUI.Controls;

public class MenuFlyoutItemBase : TemplatedControl
{
    static MenuFlyoutItemBase()
    {
        FocusableProperty.OverrideDefaultValue<MenuFlyoutItemBase>(true);
    }

    internal bool IsContainerFromTemplate { get; set; }

    internal FAMenuFlyoutPresenter InternalParent { get; set; }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);

        InternalParent.PointerEnteredItem(this);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);

        InternalParent.PointerExitedItem(this);
    }
}

