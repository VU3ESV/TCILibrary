using Avalonia;

namespace ExpertElectronics.Tci.Client;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Cache CLI options before Avalonia chews them up.
        AppOptions.Parse(args);
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

internal static class AppOptions
{
    public static bool EnableTx { get; private set; }

    public static void Parse(string[] args)
    {
        foreach (var a in args)
        {
            if (a == "--enable-tx") EnableTx = true;
        }
    }
}
