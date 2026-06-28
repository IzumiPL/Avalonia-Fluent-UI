using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;

namespace AvaloniaFluentUI.Windowing;

internal class SplashScreenContext
{
    public SplashScreenContext(IApplicationSplashScreen splash)
    {
        SplashScreen = splash;
    }

    public IApplicationSplashScreen SplashScreen { get; }

    public bool HasShownSplashScreen { get; set; }

    public AppSplashScreen Host
    {
        get => _splashHost;
        set
        {
            _splashHost = value;
            _splashHost.SplashScreen = SplashScreen;
        }
    }

    public async Task RunJobs()
    {
        _splashCTS = new CancellationTokenSource();
        await SplashScreen.RunTasks(_splashCTS.Token);
        _splashCTS?.Dispose();
        _splashCTS = null;
    }

    public void TryCancel()
    {
        _splashCTS?.Cancel();
        _splashCTS?.Dispose();
        _splashCTS = null;
    }

    private AppSplashScreen _splashHost;
    private CancellationTokenSource _splashCTS;
}

public class AppSplashScreen : TemplatedControl
{
    public IApplicationSplashScreen SplashScreen { get; set; }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (SplashScreen != null && SplashScreen.SplashScreenContent != null)
        {
            // User set content has priority
            var cp = e.NameScope.Find<ContentPresenter>("ContentHost");
            cp.Content = SplashScreen.SplashScreenContent;
        }
    }
}
