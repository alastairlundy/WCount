using System;

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Counters;

using Avalonia;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using WCountUI.DependencyInjection.Extensions;
using WCountUI.XPlatApp.ViewModels;
using WCountUI.XPlatApp.Views;

namespace WCountUI.XPlatApp.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddWCountUI();
        
        IServiceProvider provider = collection.BuildServiceProvider();
        
        return AppBuilder.Configure<App>(()=> new App(provider))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}