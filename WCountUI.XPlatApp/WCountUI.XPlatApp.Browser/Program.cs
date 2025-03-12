using System;
using System.Runtime.Versioning;

using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Counters;

using Avalonia;
using Avalonia.Browser;

using Microsoft.Extensions.DependencyInjection;
using WCountUI.DependencyInjection.Extensions;
using WCountUI.XPlatApp;
using WCountUI.XPlatApp.ViewModels;
using WCountUI.XPlatApp.Views;

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
        .WithInterFont()
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddWCountUI();

        
        IServiceProvider provider = collection.BuildServiceProvider();

        return AppBuilder.Configure<App>(() => new App(provider));
    }
}