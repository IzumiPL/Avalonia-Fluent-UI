using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// CommandBar used for a <see cref="CommandBarFlyout"/>
/// </summary>
/// <remarks>
/// This class should be treated as internal to AvaloniaFluentUI and not used outside of 
/// the CommandBarFlyout implementations.
/// </remarks>
[TemplatePart(MORE_BUTTON, typeof(Button))]
public class CommandBarFlyoutCommandBar : CommandBar
{
    // As said in the Template, this is a modified version of whats in WinUI b/c the WinUI version
    // is stupid. They have two popups that are blended to make this control - one for the flyout,
    // and one with the CommandBar. Instead of doing that, which would just be a giant headache,
    // it's combined into one Popup (the CommandBar Popup is removed and its all contained in one, 
    // and we just show/hide the overflow as necessary)
    // I genuinely think this is because some legacy aspect of the AppBar requires a popup to be
    // present, b/c I cannot think of any reason for this design. Anyway, things are different, but
    // the end result behavior should still be the same (or very close to it)
    // One drawback, is we always open down, at least for now.
    
    private List<Control>? _horizontallyAccessibleControls;
    private List<Control>? _verticallyAccessibleControls;

    private Button? _moreButton;
    private CommandBarFlyout? _owningFlyout;

    private const string MORE_BUTTON = "MoreButton";

    public CommandBarFlyoutCommandBar()
    {
        // Yes, all this is done in the ctor in WinUI

        // Treated as Loaded Event
        AttachedToVisualTree += (_, _) =>
        {
            //UpdateUI(!_commandBarFlyoutIsOpening);

            // This ensures that even in Transient ShowMode, focus is still directed into the Flyout, which technically
            // goes against the description of Transient mode, but it's what WinUI does, so whatever
            // Logic is modified...
            var commands = PrimaryCommands.Count > 0 ? PrimaryCommands : (SecondaryCommands.Count > 0 ? SecondaryCommands : null);

            if (commands != null)
            {
                // post this to the dispatcher so it's delayed, otherwise we'll take focus before we actually open
                // In case of TextCommandBarFlyout, this will end up clearing the Textbox selection because the 
                // flyout isn't open yet, but we pulled focus
                Dispatcher.UIThread.Post(() =>
                {
                    if (PrimaryCommands.Count > 0)
                    {
                        bool handled = false;
                        for (int i = 0; i < PrimaryCommands.Count; i++)
                        {
                            if (IsControlFocusable(PrimaryCommands[i] as Control))
                            {
                                if (PrimaryCommands[i] is InputElement ie)
                                {
                                    ie.Focus();
                                }
                                handled = true;
                                break;
                            }
                        }

                        if (!handled)
                        {
                            if (_moreButton != null && _moreButton.IsVisible)
                            {
                                _moreButton.Focus();
                            }
                        }
                    }
                    else
                    {
                        if (_moreButton != null && _moreButton.IsVisible)
                        {
                            _moreButton.Focus();
                        }
                    }

                }, DispatcherPriority.Loaded);
            }
        };

        Closing += (_, _) =>
        {
            if (_owningFlyout != null && _owningFlyout.IsOpen)
            {
                if (_owningFlyout.AlwaysExpanded)
                {
                    // Don't close the secondary commands list when the flyout is AlwaysExpanded
                    IsOpen = true;
                }
            }
        };

        PrimaryCommands.CollectionChanged += (_, _) =>
        {
            PopulateAccessibleControls();
        };

        SecondaryCommands.CollectionChanged += (_, _) =>
        {
            PopulateAccessibleControls();
        };
    }

    protected override Type StyleKeyOverride => typeof(CommandBarFlyoutCommandBar);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _moreButton = e.NameScope.Find<Button>(MORE_BUTTON);

        PopulateAccessibleControls();
    }

    private void PopulateAccessibleControls()
    {
        if (_horizontallyAccessibleControls == null)
        {
            _horizontallyAccessibleControls = new List<Control>();
            _verticallyAccessibleControls = new List<Control>();
        }
        else
        {
            _horizontallyAccessibleControls.Clear();
            _verticallyAccessibleControls?.Clear();
        }

        for (int i = 0; i < PrimaryCommands.Count; i++)
        {
            if (PrimaryCommands[i] is Control c)
            {
                _horizontallyAccessibleControls.Add(c);
                _verticallyAccessibleControls?.Add(c);
            }
        }

        if (_moreButton != null)
        {
            _horizontallyAccessibleControls.Add(_moreButton);
            _verticallyAccessibleControls?.Add(_moreButton);
        }

        for (int i = 0; i < SecondaryCommands.Count; i++)
        {
            if (SecondaryCommands[i] is Control c)
            {
                _verticallyAccessibleControls?.Add(c);
            }
        }
    }

    protected override void OnKeyDown(KeyEventArgs args)
    {
        if (args.Handled)
            return;

        switch (args.Key)
        {
            case Key.Tab:
                var current = TopLevel.GetTopLevel(_owningFlyout?.Target)?.FocusManager.GetFocusedElement();

                if (current == _moreButton)
                {
                    if (SecondaryCommands.Count > 0 && !IsOpen)
                    {
                        // Ensure the secondary commands flyout is open ...
                        IsOpen = true;
                    }

                    for (int i = 0; i < SecondaryCommands.Count; i++)
                    {
                        if (IsControlFocusable(SecondaryCommands[i] as Control))
                        {
                            if (SecondaryCommands[i] is InputElement ie)
                            {
                                ie.Focus(NavigationMethod.Tab);
                            }
                            args.Handled = true;
                            break;
                        }
                    }
                }

                if (!args.Handled && current != null && current is ICommandBarElement element)
                {
                    if (PrimaryCommands.Contains(element))
                    {
                        // Despite calling IsOpen above, apparently the SecondaryCommands aren't yet visible
                        // and added to the tree, which means the below will fail to move focus and it will take
                        // two tabs to actually move the focus on the first time. So we use this workaround

                        bool neededOpen = !IsOpen;
                        if (SecondaryCommands.Count > 0 && !IsOpen)
                        {
                            // Ensure the secondary commands flyout is open ...
                            IsOpen = true;
                        }

                        void FocusFirstSecondary()
                        {
                            for (int i = 0; i < SecondaryCommands.Count; i++)
                            {
                                if (IsControlFocusable(SecondaryCommands[i] as Control))
                                {
                                    if (SecondaryCommands[i] is InputElement ie)
                                    {
                                        ie.Focus(NavigationMethod.Tab);
                                    }
                                    args.Handled = true;
                                    break;
                                }
                            }
                        }

                        if (neededOpen)
                        {
                            Dispatcher.UIThread.Post(FocusFirstSecondary, DispatcherPriority.Render);
                        }
                        else
                        {
                            FocusFirstSecondary();
                        }
                    }
                    else if (current is ICommandBarElement ce && SecondaryCommands.Contains(ce))
                    {
                        for (int i = 0; i < PrimaryCommands.Count; i++)
                        {
                            if (IsControlFocusable(PrimaryCommands[i] as Control))
                            {
                                if (PrimaryCommands[i] is InputElement ie)
                                {
                                    ie.Focus(NavigationMethod.Tab);
                                }
                                args.Handled = true;
                                break;
                            }
                        }

                        if (!args.Handled)
                        {
                            if (_moreButton != null && _moreButton.IsVisible)
                            {
                                _moreButton.Focus(NavigationMethod.Tab);
                                args.Handled = true;
                            }
                        }
                    }
                }

                break;

            case Key.Right:
            case Key.Left:
            case Key.Down:
            case Key.Up:

                // INavigableContainer handles everything inside the StackPanel and we can't get around
                // that without using Preview Key handlers, which is a no-go
                // So if we make it here, we're at a point where INavigableContainer won't move the focus
                // so we need to handle it. Logic still adapted from WinUI

                // WinUI behavior, Left/Right only navigate in PrimaryCommands
                // Up/down will iterate through all commands

                bool isLeft = args.Key == Key.Left;
                bool isRight = args.Key == Key.Right;
                bool isUp = args.Key == Key.Up;
                bool isDown = args.Key == Key.Down;

                var accessibleControls = (isUp || isDown) ? _verticallyAccessibleControls : _horizontallyAccessibleControls;
                int startIndex = (isLeft || isUp) ? accessibleControls.Count - 1 : 0;
                int endIndex = (isLeft || isUp) ? -1 : accessibleControls.Count;
                int deltaIndex = (isLeft || isUp) ? -1 : 1;
                bool shouldLoop = (isUp || isDown);
                Control? focused = null;
                int focusedIndex = -1;

                for (int i = startIndex;
                    (i != endIndex || shouldLoop) ||
                    (focusedIndex > 0 && i == focusedIndex); i += deltaIndex)
                {
                    if (i == endIndex)
                    {
                        if (focused != null)
                        {
                            i = startIndex;
                        }
                        else
                        {
                            break;
                        }
                    }

                    var control = accessibleControls[i];

                    if (focused == null)
                    {
                        if (control.IsFocused)
                        {
                            focused = control;
                            focusedIndex = i;
                        }
                    }
                    else if (IsControlFocusable(control))
                    {
                        if (control is ICommandBarElement ele)
                        {
                            if (SecondaryCommands.Contains(ele) && !IsOpen)
                            {
                                IsOpen = true;
                            }
                        }

                        control.Focus(NavigationMethod.Directional);
                        args.Handled = true;
                        break;
                    }
                }

                if (!args.Handled)
                {
                    args.Handled = true;
                }
                break;
        }

        base.OnKeyDown(args);
    }

    private bool IsControlFocusable(Control? control)
    {
        return control != null && control.IsVisible && control.IsEnabled && control.Focusable;
    }

    internal void SetOwningFlyout(CommandBarFlyout f)
    {
        _owningFlyout = f;
    }
}
