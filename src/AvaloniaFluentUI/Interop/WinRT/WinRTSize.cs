using System.Runtime.InteropServices;

namespace AvaloniaFluentUI.Interop.WinRT;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct WinRTSize
{
    public float Width;
    public float Height;
}
