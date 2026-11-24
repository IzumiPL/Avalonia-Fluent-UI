// Adapted from TerraFX.Interop.Windows, MIT license

namespace AvaloniaFluentUI.Controls.Interop.Win32;

/// <summary>
/// Flags used by <see cref="DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_POLICY"/> to specify how the
/// non-client area of a window is rendered by DWM.
/// </summary>
internal enum DWMNCRENDERINGPOLICY
{
    /// <summary>
    /// The non-client area rendering policy is derived from the window style (default).
    /// </summary>
    DWMNCRP_USEWINDOWSTYLE = 0,

    /// <summary>
    /// The non-client area is not rendered by DWM, which also removes the window frame border.
    /// </summary>
    DWMNCRP_DISABLED = 1,

    /// <summary>
    /// The non-client area rendering is enabled.
    /// </summary>
    DWMNCRP_ENABLED = 2,

    DWMNCRP_LAST = 3
}
