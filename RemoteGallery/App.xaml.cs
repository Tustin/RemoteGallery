using Prism.Ioc;
using Prism.Unity;
using RemoteGallery.Configuration;
using RemoteGallery.Services;
using RemoteGallery.Views;
using Serilog;
using System.IO;
using System.Windows;

namespace RemoteGallery;

public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
            .WriteTo.Debug()
#endif
            .WriteTo.File(Path.Join(AppConfiguration.LogsDirectory, "log-.txt"),
                rollingInterval: RollingInterval.Month)
            .CreateLogger();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<ITmdbResolverService, TmdbResolverService>();
        containerRegistry.RegisterSingleton<IFtpHandler, FtpHandler>();
    }

}
