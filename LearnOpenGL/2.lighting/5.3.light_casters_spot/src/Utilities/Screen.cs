using Silk.NET.Windowing;

namespace LearnSilkNET.Utilities;

public static class Screen
{
    public static int Width => _window.Size.X;
    public static int Height => _window.Size.Y;

    private static IWindow _window = null!;

    public static void Initialize(IWindow window)
    {
        _window = window;
    }
}
